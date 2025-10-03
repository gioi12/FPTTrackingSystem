using DataTranferObjects.Staff.Response;
using FPTTrackingSystem.Wrappers;
using Repositories.Staff;

namespace FPTTrackingSystem.Services.Staff
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMajorRepository _majorRepository;
        public SemesterService(ISemesterRepository semesterRepository, IMajorRepository majorRepositoy)
        {
            _semesterRepository = semesterRepository;
            _majorRepository = majorRepositoy;
        }

        public async Task<ApiResponse<SemesterActiveRes>> GetSemesterActiveAndMajors()
        {
            var semester =await _semesterRepository.findActive();
            var majors = await _majorRepository.findAll();
            SemesterActiveRes se = new SemesterActiveRes()
            {
                Id = semester.Id,
                Name = semester.Name,
                Description = semester.Description,
                majors = majors.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Code
                }).Cast<object>().ToList()
            };
            return ApiResponse<SemesterActiveRes>.Success(se);
        }
    }
}
