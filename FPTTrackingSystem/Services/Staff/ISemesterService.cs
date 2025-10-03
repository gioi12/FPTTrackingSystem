using DataTranferObjects.Staff.Response;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff
{
    public interface ISemesterService
    {
        Task<ApiResponse<SemesterActiveRes>> GetSemesterActiveAndMajors();
    }
}
