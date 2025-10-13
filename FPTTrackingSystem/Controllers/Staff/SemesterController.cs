using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Semester;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        private readonly FpttrackingSystemContext _context;
        private readonly ILogService _logService;

        public SemesterController(ISemesterService semesterService, FpttrackingSystemContext context, ILogService logService)
        {
            _semesterService = semesterService;
            _context = context;
            _logService = logService;
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("v1/Staff/semester")]
        public async Task<object> GetSemesterInActive()
        {
            return Ok(await _semesterService.GetSemesterActiveAndMajors());
        }

        [HttpPost("v1/Staff/semester/create")]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterCreateRequest request)
        {
            if (!DateOnly.TryParse(request.StartAt, out var startAt) ||
       !DateOnly.TryParse(request.EndAt, out var endAt))
            {
                return BadRequest("Ngày không hợp lệ (định dạng phải là yyyy-MM-dd).");
            }
            var isOverlap = await _semesterService.IsOverlappingAsync(startAt, endAt);
            if (isOverlap)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Khoảng thời gian của kỳ học này đã tồn tại trong hệ thống."
                });
            }

            var result = await _semesterService.CreateSemesterAsync(request);
            return Ok(new
            {
                Status = 200,
                Message = "Tạo kỳ học thành công!",
                Data = result
            });
        }

        [HttpGet("v1/Staff/semester/getAll")]
        public async Task<IActionResult> GetAllSemesters()
        {
            try
            {
                var semesters = await _semesterService.GetAllSemestersAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Lấy danh sách kỳ học thành công!",
                    data = semesters
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = $"Lỗi khi lấy danh sách kỳ học: {ex.Message}"
                });
            }
        }

        [HttpGet("v1/Staff/semester/getSemesterBy/{id}")]
        public async Task<IActionResult> GetSemesterById(int id)
        {
            var semester = await _semesterService.GetSemesterByIdAsync(id);

            if (semester == null)
            {
                return NotFound(ApiResponse<object>.Fail("Không tìm thấy học kỳ", 404));
            }

            return Ok(ApiResponse<SemesterDTO>.Success(semester, "Lấy học kỳ thành công"));
        }

        [HttpPost("v1/Staff/semester/{id}")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterUpdateRequest semesterData)
        {
            try
            {
                var result = await _semesterService.UpdateSemesterAsync(id, semesterData);
                return Ok(ApiResponse<SemesterDTO>.Success(result, "Cập nhật học kỳ thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        [HttpPost("v1/Staff/semester/vacation")]
        public async Task<IActionResult> UpdateVacationWeeks([FromBody] UpdateVacationWeeksRequest request)
        {
            try
            {
                if (request == null || request.Weeks == null || request.Weeks.Count == 0)
                    return BadRequest(ApiResponse<string>.Fail("Dữ liệu cập nhật không hợp lệ"));

                var semester = await _context.Semesters
                    .Include(s => s.SemesterWeeks)
                    .FirstOrDefaultAsync(s => s.Id == request.SemesterId);

                if (semester == null)
                    return Ok(ApiResponse<string>.Fail("Không tìm thấy kỳ học"));

                foreach (var week in semester.SemesterWeeks)
                {
                    var updateWeek = request.Weeks.FirstOrDefault(x => x.WeekNumber == week.WeekNumber);
                    if (updateWeek != null)
                    {
                        week.IsVacation = updateWeek.IsVacation;
                    }
                }

                int learnWeekCounter = 0;
                var orderedWeeks = semester.SemesterWeeks.OrderBy(w => w.WeekNumber).ToList();

                foreach (var week in orderedWeeks)
                {
                    if (week.IsVacation == true)
                    {
                        week.WeekLearn = null;
                    }
                    else
                    {
                        learnWeekCounter++;
                        week.WeekLearn = learnWeekCounter;
                    }

                    _context.Entry(week).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                _logService.AddLog(new Log
                {
                    Name = "Cập nhật tuần nghỉ",
                    EntityName = "Semester",
                    EntityId = semester.Id,
                    Action = "UPDATE",
                    Description = $"Cập nhật tuần nghỉ/học cho kỳ '{semester.Name}' (ID: {semester.Id}) - Tuần nghỉ: {orderedWeeks.Count - learnWeekCounter}",
                    UserId = 1, 
                    CreateAt = DateTime.Now
                });

                return Ok(ApiResponse<string>.Success(null, "Cập nhật tuần nghỉ và tuần học thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError(ex.Message));
            }
        }

    }
}
