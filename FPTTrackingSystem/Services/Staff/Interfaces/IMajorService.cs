using DataTranferObjects.Staff.Group;
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
        Task<PagedData<MajorCategoryDTO>> GetAllCoursesPagedAsync(int page, int pageSize);
        Task<List<MajorCategoryDTO>> GetAllCoursesAsync();
        Task<MajorCategoryDTO?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MajorCategoryDTO dto);
        Task<bool> UpdateAsync(MajorCategoryDTO dto);

    }
}
