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
        public DeliverableService(IDeliverableRepository deliverableRepository, ISemesterRepository semesterRepository, IWebHostEnvironment env,IAttachmentRepository attachmentRepository)
        {
            _deliverableRepository = deliverableRepository;
            _semesterRepository = semesterRepository;
            _env = env;
            _attachmentRepository = attachmentRepository;
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

        public async Task<string> UploadFileMilestone(IFormFile file,int groupId,int deliverableId)
        {
            var deli = await _deliverableRepository.GetById(deliverableId);
            if (deli == null) throw new ValidationException("Not found delivery");
            if (deli.DeliverableGroups.Count == 0)
            {
                deli.DeliverableGroups.Add(new DeliverableGroup()
                {
                    DeliverableId = deliverableId,
                    GroupId = groupId,
                    Status = "Pending"
                });
                await _deliverableRepository.UpdateDeliverable(deli);
            }
            string path = await FileUploadUtils.UploadFileAsync(file, (int)FileEnum.Deliverable, _env);
            // 1. Milestone(Deliverable) , 2 Task , 3 Groups (Documents)
            string entityName = FileUploadUtils.GetEntityName((int)FileEnum.Deliverable);
            Attachment attachment = new Attachment()
            {
                CreateAt = DateTime.Now,
                FileName = file.FileName,
                FilePath = path,
                EntityName = entityName,
                EntityId = deliverableId,
                GroupId = groupId
            };
            await _attachmentRepository.AddAttachment(attachment);
            return path;
        }
    }
}
