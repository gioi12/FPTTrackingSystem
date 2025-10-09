using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Semester;
using FPTTrackingSystem.Services.Staff;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        
        public SemesterController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
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
    }
}
