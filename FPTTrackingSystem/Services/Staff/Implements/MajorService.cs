using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Repositories.Staff;

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
                Code = m.Code,
                Name = m.Name
            }).ToList();

            return ApiResponse<List<MajorResponse>>.Success(majorResponses, "Lấy danh sách ngành thành công", 200);
        }

    }
}
