using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common.Interfaces;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class UploadService : IUploadService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        public UploadService(IAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;   
        }
        public string UploadFile(IFormFile file, int type, [FromServices] IWebHostEnvironment env)
        {
            //Attachment attachment = new Attachment()
            //{
            //    CreateAt = DateTime.Now,

            //};
            return null;
        }
    }
}
