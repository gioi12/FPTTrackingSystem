using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using DataTranferObjects.Staff.Semester;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff
{
    public interface ISemesterService
    {
        Task<ApiResponse<SemesterActiveRes>> GetSemesterActiveAndMajors();
        Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest request);
        Task<bool> IsOverlappingAsync(DateOnly start, DateOnly end);
    }
}
