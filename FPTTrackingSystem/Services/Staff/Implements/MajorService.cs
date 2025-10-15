using DataTranferObjects.Staff.Major;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Repositories.Staff.Interfaces;

namespace FPTTrackingSystem.Services.Staff.Implementations
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;

        public MajorService(IMajorRepository majorRepository)
        {
            _majorRepository = majorRepository;
        }

        public async Task<ApiResponse<List<MajorResponse>>> GetAllMajors()
        {
            var majors = await _majorRepository.findAll();

            if (majors == null || majors.Count == 0)
            {
                return ApiResponse<List<MajorResponse>>.Fail("Không có ngành nào được tìm thấy", 404);
            }
            var majorResponses = majors.Select(m => new MajorResponse
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();

            return ApiResponse<List<MajorResponse>>.Success(majorResponses, "Lấy danh sách ngành thành công", 200);
        }

        public async Task<List<MajorDTO>> GetAllMajorAndCategoriesAsync()
        {
            var majors = await _majorRepository.getAllMajorAndCode();

            var result = majors.Select(m => new MajorDTO
            {
                Id = m.Id,
                Name = m.Name,
                Status = m.Status,
                MajorCategories = m.MajorCategories.Select(c => new MajorCategoryDTO
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<MajorCategoryDTO>> GetAllCoursesAsync()
        {
            var entities = await _majorRepository.getAllCourse();

            return entities.Select(c => new MajorCategoryDTO
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name
            }).ToList();
        }

    }
}
