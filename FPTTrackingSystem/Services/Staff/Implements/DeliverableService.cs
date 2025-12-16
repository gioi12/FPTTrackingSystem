using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using Mapster;
using Repositories.Common.Interfaces;
using Repositories.Staff.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FPTTrackingSystem.Services.Staff.Implementations
{
    public class DeliverableService : IDeliverableSevice
    {
        private readonly IDeliverableRepository _deliverableRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly AuthUtils _authUtils;
        public DeliverableService(IDeliverableRepository deliverableRepository, ISemesterRepository semesterRepository, IWebHostEnvironment env,IAttachmentRepository attachmentRepository,AuthUtils authUtils,IGroupRepository groupRepository)
        {
            _deliverableRepository = deliverableRepository;
            _semesterRepository = semesterRepository;
            _env = env;
            _attachmentRepository = attachmentRepository;
            _authUtils = authUtils;
            _groupRepository = groupRepository;
        }
        public async Task<List<DeliverableRes>> GetDeliverableByCodeAndSemester(int semesterId, int code)
        {
            var list = await _deliverableRepository.GetByCodeAndSemester(code, semesterId);
            var semester = await _semesterRepository.GetSemesterByIdAsync(semesterId);
            var res = list.Adapt<List<DeliverableRes>>();
            var holidays = semester.SemesterVacations
               .Where(x => x.StartAt.HasValue && x.EndAt.HasValue)
               .Select(x => (x.StartAt.Value, x.EndAt.Value))
               .ToList();
            foreach (var item in res)
            {
                item.StartAt = semester.StartAt;
                item.EndAt = item.Deadline != null ? DateTimeUtils.GetTargetDate(
                    item.Deadline,
                    (DateTime)semester.StartAt,
                    holidays
                ) : null;
            }
            return res;
        }

        public async Task<List<GroupDeliverableRes>> GetDeliverableByGroupId(int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null) throw new ValidationException("Not found group");
            if(!group.GroupUsers.Any(x=>x.UserId == user.Id)) throw new ValidationException("Not permission");
            var list = await _deliverableRepository.GetByCodeAndSemesterGroup((int)group.MajorId, (int)group.SemesterId,groupId);
            var semester = await _semesterRepository.GetSemesterByIdAsync((int)group.SemesterId);
            var holidays = semester.SemesterVacations
                .Where(x => x.StartAt.HasValue && x.EndAt.HasValue)
                .Select(x => (x.StartAt.Value, x.EndAt.Value))
                .ToList();
            foreach (var item in list)
            {
                item.StartAt = semester.StartAt;
                item.EndAt = item.Deadline != null ? DateTimeUtils.GetTargetDate(item.Deadline, (DateTime)semester.StartAt, holidays) : null;
            }
            return list;
        }

        public async Task<string> UploadFileMilestoneItem([Required]IFormFile file,int groupId,int deliveryItemId,string semester)
        {
            var itemDeli = await _deliverableRepository.GetItemByItemId(deliveryItemId);
            var user = await _authUtils.GetUserInfoFromCookie();
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new ValidationException("Not found group");
            if (itemDeli == null) throw new ValidationException("Not found delivery");
           
            string path = await FileUploadUtils.UploadFileAsync(file, (int)FileEnum.DeliverableItem, _env,semester,"Group"+groupId);
            // 1. Milestone(Deliverable) , 2 Task , 3 Groups (Documents)
            string entityName = FileUploadUtils.GetEntityName((int)FileEnum.DeliverableItem);
            Entities.Models.Attachment attachment = new Entities.Models.Attachment()
            {
                CreateAt = DateTime.Now,
                FileName = file.FileName,
                FilePath = path,
                EntityName = entityName,
                EntityId = deliveryItemId,
                GroupId = groupId,
                UserId = (int)user.Id,
                IsDownload = false
            };
            await _attachmentRepository.AddAttachment(attachment);
            if (itemDeli.Deliverable.DeliverableGroups.Count == 0)
            {
                var deli = itemDeli.Deliverable;
                deli.DeliverableGroups.Add(new DeliverableGroup()
                {
                    DeliverableId = deli.Id,
                    GroupId = groupId,
                    Status = "Pending"
                });
                await _deliverableRepository.UpdateDeliverable(deli);
            }
            else
            {
                var deli = itemDeli.Deliverable;
                var groupDeli = deli.DeliverableGroups.FirstOrDefault(x => x.Status == ProgressEnum.Rejected);
                if (groupDeli != null)
                {
                    groupDeli.Status = ProgressEnum.Pending;
                }
                await _deliverableRepository.UpdateDeliverable(deli);

            }
            return path;
        }

        public async Task<DeliverableDetailRes> GetDeliverableByIdAndGroupId(int groupId, int deliverableId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new ValidationException("Not found group");

            if (group.GroupUsers == null || !group.GroupUsers.Any(x => x.UserId == user.Id))
                throw new ValidationException("Not permission");

            var deliverable = await _deliverableRepository.GetById(deliverableId);
            if (deliverable == null)
                throw new ValidationException("Not found deliverable");

            var semester = await _semesterRepository.GetSemesterByIdAsync((int)group.SemesterId);

            var res = deliverable.Adapt<DeliverableDetailRes>();
            var holidays = semester.SemesterVacations
                .Where(x => x.StartAt.HasValue && x.EndAt.HasValue)
                .Select(x => (x.StartAt.Value, x.EndAt.Value))
                .ToList();
            res.StartAt = semester.StartAt;
            res.EndAt = res.Deadline != null
                ? DateTimeUtils.GetTargetDate(res.Deadline, (DateTime)semester.StartAt,holidays)
                : null;
   
            var entityName = FileUploadUtils.GetEntityName((int)FileEnum.DeliverableItem);
            var allAttachments = await _attachmentRepository.GetAttachmentsByIds(
                entityName, res.DeliveryItems.Select(x => x.Id).ToList(), groupId
            );

            foreach (var item in res.DeliveryItems)
            {
                var attachmentsForItem = allAttachments
                    .Where(a => a.EntityId == item.Id)
                    .Select(a => new attachmentItemRes
                    {
                        id = a.Id,
                        fileName = a.FileName,
                        path = a.FilePath,
                        createAt = a.CreateAt,
                        userName = a.User.Fullname,
                        isDownload = a.IsDownload == null ? false : (bool)a.IsDownload
                    })
                    .ToList();

                item.attachments = attachmentsForItem;
            }

            return res;
        }

        public async Task<string> ConfirmDeliverable(int groupId, int deliverableId , string? note)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new ValidationException("Not found group");

            if (group.GroupUsers == null || !group.GroupUsers.Any(x => x.UserId == user.Id))
                throw new ValidationException("Not permission");

            var deliverable = await _deliverableRepository.GetById(deliverableId);
            if (deliverable == null)
                throw new ValidationException("Not found deliverable");
            
            var semester = await _semesterRepository.GetSemesterByIdAsync((int)group.SemesterId);
            var item = deliverable.DeliverableGroups.Where(x => x.GroupId == groupId && x.DeliverableId == deliverableId).FirstOrDefault();
            if(item.Status == ProgressEnum.Unsubmitted)
            {
                throw new ValidationException("Not submitted");
            }
            var holidays = semester.SemesterVacations
               .Where(x => x.StartAt.HasValue && x.EndAt.HasValue)
               .Select(x => (x.StartAt.Value, x.EndAt.Value))
               .ToList();
            var deadlineAt = deliverable.Deadline != null
                ? (DateTime?)DateTimeUtils.GetTargetDate(deliverable.Deadline, (DateTime)semester.StartAt, holidays)
                : null;

            string statusUpdate = null;
            if(DateTime.Now > deadlineAt)
            {
                statusUpdate = ProgressEnum.Late;
            }
            else
            {
                statusUpdate = ProgressEnum.Confirmed;
            }
            item.Status = statusUpdate;
            item.Note = note;
            await _deliverableRepository.UpdateDeliverable(deliverable);

            return statusUpdate;
        }

        public async Task<List<DeliverableGroupDetailDTO>> GetDeliverableGroupsByGroupIdAsync(int groupId)
        {
            var deliverableGroups = await _deliverableRepository.GetDeliverableGroupsByGroupIdAsync(groupId);

            return deliverableGroups
                .Where(dg => dg.Deliverable != null && dg.Deliverable.IsActive == true)
                .Select(dg => new DeliverableGroupDetailDTO
                {
                    Id = dg.Deliverable.Id,
                    Name = dg.Deliverable.Name,
                    Description = dg.Deliverable.Description,
                    Deadline = dg.Deliverable.Deadline,
                    CreateAt = dg.Deliverable.Milestone?.CreateAt
                })
                .ToList();
        }

        public async System.Threading.Tasks.Task DeleteFileMilestoneItem(int attachmentId)
        {
            var attachment = await _attachmentRepository.GetAttachmentById(attachmentId);
            if(attachment == null) throw new ValidationException("Not found attachment");
            await _attachmentRepository.DeleteAttachment(attachment);
        }

        public async System.Threading.Tasks.Task MarkDownload(int attachmentId)
        {
            var attachment = await _attachmentRepository.GetAttachmentById(attachmentId);
            if(attachment == null) throw new ValidationException("Not found attachment");
            attachment.IsDownload = true;
            await _attachmentRepository.UpdateAttachment(attachment);
        }

        public async Task<string> RejectDeliverable(int groupId, int deliverableId,string? note)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new ValidationException("Not found group");

            if (group.GroupUsers == null || !group.GroupUsers.Any(x => x.UserId == user.Id))
                throw new ValidationException("Not permission");

            var deliverable = await _deliverableRepository.GetById(deliverableId);
            if (deliverable == null)
                throw new ValidationException("Not found deliverable");

            var semester = await _semesterRepository.GetSemesterByIdAsync((int)group.SemesterId);
            var item = deliverable.DeliverableGroups.Where(x => x.GroupId == groupId && x.DeliverableId == deliverableId).FirstOrDefault();
            if (item.Status == ProgressEnum.Unsubmitted)
            {
                throw new ValidationException("Not submitted");
            }

            item.Note = note;
            item.Status = ProgressEnum.Rejected;
            await _deliverableRepository.UpdateDeliverable(deliverable);

            return ProgressEnum.Rejected;
        }

  
    }
}
