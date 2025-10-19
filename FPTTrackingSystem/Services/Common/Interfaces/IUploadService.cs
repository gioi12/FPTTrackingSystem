using DataTranferObjects.Common.Request;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadFile(IFormFile file, UploadRequest request, [FromServices] IWebHostEnvironment env);
    }
}
