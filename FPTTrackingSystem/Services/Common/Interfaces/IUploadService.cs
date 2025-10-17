using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface IUploadService
    {
        string UploadFile(IFormFile file, int type, [FromServices] IWebHostEnvironment env);
    }
}
