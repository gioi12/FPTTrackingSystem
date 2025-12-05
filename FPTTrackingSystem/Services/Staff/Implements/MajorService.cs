using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Major;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using iText.Kernel.Geom;
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

        public async Task<PagedData<MajorCategoryDTO>> GetAllCoursesPagedAsync(int page, int pageSize)
        {
            var data = await _majorRepository.GetAllCoursePagedAsync(page, pageSize);

            return new PagedData<MajorCategoryDTO>
            {
                Items = data.Items.Select(c => new MajorCategoryDTO
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    Size = c.Size
                }).ToList(),
                Total = data.Total
            };
        }

        public async Task<MajorCategoryDTO?> GetByIdAsync(int id)
        {
            var entity = await _majorRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return new MajorCategoryDTO
            {
                Name = entity.Name,
                Code = entity.Code,
                IsActive = entity.IsActive,
                Size = entity.Size
            };
        }

        public async Task<bool> CreateAsync(MajorCategoryDTO dto)
        {
            var entity = new MajorCategory
            {
                Name = dto.Name,
                Code = dto.Code,
                IsActive = dto.IsActive,
                Size = dto.Size
            };
            return await _majorRepository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(MajorCategoryDTO dto)
        {
            var existing = await _majorRepository.GetByIdAsync(dto.Id);
            if (existing == null) return false;

            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.IsActive = dto.IsActive;
            existing.Size = dto.Size;   
            return await _majorRepository.UpdateAsync(existing);
        }

    }
}
