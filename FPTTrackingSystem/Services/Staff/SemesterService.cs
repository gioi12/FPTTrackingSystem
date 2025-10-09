using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using DataTranferObjects.Staff.Semester;
using Entities.Models;
using FPTTrackingSystem.Hepler;
using FPTTrackingSystem.Wrappers;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff;

namespace FPTTrackingSystem.Services.Staff
{
    public class SemesterService : ISemesterService
    {
        private readonly FpttrackingSystemContext _context;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMajorRepository _majorRepository;
        public SemesterService(ISemesterRepository semesterRepository, IMajorRepository majorRepositoy, FpttrackingSystemContext context)
        {
            _semesterRepository = semesterRepository;
            _majorRepository = majorRepositoy;
            _context = context;
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

        public async Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest request)
        {
            if (!DateTime.TryParse(request.StartAt, out var startAtDateTime) ||
                !DateTime.TryParse(request.EndAt, out var endAtDateTime))
            {
                throw new Exception("Ngày không hợp lệ. Định dạng phải là yyyy-MM-dd.");
            }

            var startAt = DateOnly.FromDateTime(startAtDateTime);
            var endAt = DateOnly.FromDateTime(endAtDateTime);

            if (startAt >= endAt)
            {
                throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");
            }

            var activeSemester = await _context.Semesters.FirstOrDefaultAsync(s => s.IsActive ?? false);
            if (activeSemester != null)
            {
                activeSemester.IsActive = false;
            }

            var semester = new Semester
            {
                Name = request.Name,
                StartAt = startAtDateTime,
                EndAt = endAtDateTime,
                Description = request.Description,
                IsActive = true
            };

            try
            {
                _context.Semesters.Add(semester);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu dữ liệu: {ex.InnerException?.Message ?? ex.Message}");
            }

            var weeks = SemesterHelper.GetWeeks(startAt, endAt);
            return new SemesterDTO
            {
                Id = semester.Id,
                Name = semester.Name ?? string.Empty,
                StartAt = semester.StartAt ?? default,
                EndAt = semester.EndAt ?? default,
                Weeks = weeks,
                IsActive = true,
            };
        }


        public async Task<bool> IsOverlappingAsync(DateOnly start, DateOnly end)
        {
            var startDateTime = start.ToDateTime(TimeOnly.MinValue);
            var endDateTime = end.ToDateTime(TimeOnly.MaxValue);
            return await _context.Semesters.AnyAsync(s =>
      s.StartAt.HasValue && s.EndAt.HasValue &&
      startDateTime <= s.EndAt.Value && endDateTime >= s.StartAt.Value);
        }

        public async Task<List<SemesterDTO>> GetAllSemestersAsync()
        {
            var semesters = await _semesterRepository.getAllSemesters();

            return semesters.Select(s => new SemesterDTO
            {
                Id = s.Id,
                Name = s.Name ?? "",
                StartAt = s.StartAt ?? default,
                EndAt = s.EndAt ?? default,
                Description = s.Description ?? "",
                IsVacation = s.IsVacation,
                IsActive = s.IsActive,
                Weeks = SemesterHelper.GetWeeks(DateOnly.FromDateTime(s.StartAt ?? default), DateOnly.FromDateTime(s.EndAt ?? default))
            }).ToList();
        }

    }
}
