using DataTranferObjects.Common.Request;
using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
            foreach (var item in res)
            {
                item.StartAt = semester.StartAt;
                item.EndAt = item.Deadline != null ? DateTimeUtils.GetDeadlineDate(item.Deadline, (List<Entities.Models.SemesterWeek>)semester.SemesterWeeks) : null;
            }
            return res;
        }

        public async Task<List<GroupDeliverableRes>> GetDeliverableByGroupId(int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null) throw new ValidationException("Not found group");
            if(!group.GroupUsers.Any(x=>x.UserId == user.Id)) throw new ValidationException("Not permission");
            var list = await _deliverableRepository.GetByCodeAndSemester((int)group.MajorId, (int)group.SemesterId);
            var semester = await _semesterRepository.GetSemesterByIdAsync((int)group.SemesterId);
            var res = list.Adapt<List<GroupDeliverableRes>>();
            foreach (var item in res)
            {
                item.StartAt = semester.StartAt;
                item.EndAt = item.Deadline != null ? DateTimeUtils.GetDeadlineDate(item.Deadline, (List<Entities.Models.SemesterWeek>)semester.SemesterWeeks) : null;
            }
            return res;
        }

        public async Task<string> UploadFileMilestoneItem([Required]IFormFile file,int groupId,int deliveryItemId)
        {
            var itemDeli = await _deliverableRepository.GetItemByItemId(deliveryItemId);
            var user = await _authUtils.GetUserInfoFromCookie();
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new ValidationException("Not found group");
            if (itemDeli == null) throw new ValidationException("Not found delivery");
           
            string path = await FileUploadUtils.UploadFileAsync(file, (int)FileEnum.DeliverableItem, _env);
            // 1. Milestone(Deliverable) , 2 Task , 3 Groups (Documents)
            string entityName = FileUploadUtils.GetEntityName((int)FileEnum.DeliverableItem);
            Attachment attachment = new Attachment()
            {
                CreateAt = DateTime.Now,
                FileName = file.FileName,
                FilePath = path,
                EntityName = entityName,
                EntityId = deliveryItemId,
                GroupId = groupId,
                UserId = (int)user.Id
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
            res.StartAt = semester.StartAt;
            res.EndAt = res.Deadline != null
                ? DateTimeUtils.GetDeadlineDate(res.Deadline, (List<Entities.Models.SemesterWeek>)semester.SemesterWeeks)
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
                        path = a.FilePath,
                        createAt = a.CreateAt,
                        userName = a.User.Fullname
                    })
                    .ToList();

                item.attachments = attachmentsForItem;
            }

            return res;
        }


    }
}
