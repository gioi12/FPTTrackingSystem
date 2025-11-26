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
            return string.Empty;
        }
    }
}
