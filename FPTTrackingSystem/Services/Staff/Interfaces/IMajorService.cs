using DataTranferObjects.Staff.Major;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IMajorService
    {
        Task<ApiResponse<List<MajorResponse>>> GetAllMajors();
        Task<List<MajorDTO>> GetAllMajorAndCategoriesAsync();
        Task<List<MajorCategoryDTO>> GetAllCoursesAsync();
    }
}
