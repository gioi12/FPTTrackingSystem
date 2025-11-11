using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Semester;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService; 
        private readonly FpttrackingSystemContext _context;
        private readonly ILogService _logService;
        private readonly AuthUtils _authUtils;

        public SemesterController(ISemesterService semesterService, FpttrackingSystemContext context, ILogService logService, AuthUtils authUtils)
        {
            _semesterService = semesterService;
            _context = context;
            _logService = logService;
            _authUtils = authUtils;
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
/*            // ✅ Check overlap
            var isOverlap = await _semesterService.IsOverlappingAsync(startAt, endAt);
            if (isOverlap)
            {
                return BadRequest(new ApiResponse<string>(400, "This semester's time range overlaps with an existing semester."));
            }
*/
            try
            {
                var result = await _semesterService.CreateSemesterAsync(request);

                return Ok(new ApiResponse<object>(200, "Semester created successfully!", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<string>(400, ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<string>(401, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(500, $"Internal server error: {ex.Message}"));
            }
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
                return Ok(ApiResponse<object>.Success(null,"Không tìm thấy học kỳ"));
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

        [HttpPost("v1/Staff/semester/sync")]
        public async Task<IActionResult> SyncSemester([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(ApiResponse<string>.Fail("Semester name is required"));

            try
            {
                var semesterResponse = await _semesterService.SyncSemesterByNameAsync(name);

                if (semesterResponse.Status != 200)
                    return BadRequest(semesterResponse);

                return Ok(semesterResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.Unauthorized(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.InternalError(ex.Message));
            }
        }

        [HttpGet("v1/Staff/semester/getSemesterByNow")]
        public async Task<IActionResult> GetCurrentSemester()
        {
            try
            {
                var semester = await _semesterService.GetSemesterByNow();

                if (semester == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Không có học kỳ nào đang hoạt động."));
                }

                return Ok(ApiResponse<Semester>.Success(semester, "Lấy học kỳ hiện tại thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpGet("v1/Staff/semester/getDeliveriesBySemester/{id}")]
        public async Task<IActionResult> GetDeliveriesBySemester(int id)
        {
            try
            {
                var semester = await _semesterService.GetDeliveriesBySemester(id);

                if (semester == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Không có học kỳ nào đang hoạt động."));
                }

                return Ok(ApiResponse<SemesterDeliveriesDTO>.Success(semester, "Lấy deliveries từ học kỳ thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpGet("v1/Staff/semester/getMilestonesBySemester/{id}")]
        public async Task<IActionResult> GetMilestonesBySemester(int id)
        {
            try
            {
                var semester = await _semesterService.GetMilestonesBySemester(id);

                if (semester == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Không có học kỳ nào đang hoạt động."));
                }

                return Ok(ApiResponse<SemesterDeliveriesDTO>.Success(semester, "Lấy Milestones từ học kỳ thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpPost("v1/Staff/semester/vacations")]
        public async Task<IActionResult> AddVacations([FromBody] List<SemesterVacationRequestDto> vacations)
        {
            var result = await _semesterService.AddVacationsAsync(vacations);
            return StatusCode(result.Status, result);
        }

        [HttpPut("v1/Staff/semester/{semesterId}/vacations")]
        public async Task<IActionResult> UpdateSemesterVacations(int semesterId,[FromBody] List<SemesterUpdateVacationRequestDto> vacationDtos)
        {
            try
            {
                var result = await _semesterService.UpdateSemesterVacationsAsync(semesterId, vacationDtos);
                return StatusCode(result.Status, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
                {
                    Status = 403,
                    Message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, new
                {
                    Status = 400,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "Internal server error.",
                    Detail = ex.Message
                });
            }
        }


        [HttpGet("v1/Staff/semester/getVacationBySemesterId/{semesterId}")]
        public async Task<IActionResult> GetVacationsBySemester(int semesterId)
        {
            var response = await _semesterService.GetVacationsBySemesterAsync(semesterId);
            return StatusCode(response.Status, response);
        }
    }
}
