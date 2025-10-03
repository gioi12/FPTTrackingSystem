using DataTranferObjects.Group;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff
{
    public interface IMajorService
    {
        Task<ApiResponse<List<MajorResponse>>> GetAllMajors();
    }
}
