using DataTranferObjects.Common.Request;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common.Interfaces;
using System.Threading.Tasks;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class UploadService : IUploadService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        public UploadService(IAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;   
        }
        public async Task<string> UploadFile(IFormFile file,UploadRequest request, [FromServices] IWebHostEnvironment env)
        {
            string path =  await FileUploadUtils.UploadFileAsync(file, request.Type, env);
            // 1. Milestone(Deliverable) , 2 Task , 3 Groups (Documents)
            string entityName = FileUploadUtils.GetEntityName(request.Type);
            Attachment attachment = new Attachment()
            {
                CreateAt = DateTime.Now,
                FileName = file.FileName,
                FilePath = path,
                EntityName = entityName,
                EntityId = request.TargetId,
                GroupId = request.GroupId
                
            };
            await _attachmentRepository.AddAttachment(attachment);
            return path;
        }
    }
}
