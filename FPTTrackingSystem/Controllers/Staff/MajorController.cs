using DataTranferObjects.Staff.Major;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/v1/Staff/")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly IMajorService _majorService;

        public MajorController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        [HttpGet("GetMajors")]  
        public async Task<IActionResult> GetAll()
        {
            var response = await _majorService.GetAllMajors();
            return StatusCode(response.Status, response);
        }

        [HttpGet("getAllCodeCourseInMajor")]
        public async Task<IActionResult> GetAllMajorsWithCategories()
        {
            try
            {
                var majors = await _majorService.GetAllMajorAndCategoriesAsync();

                if (majors == null || majors.Count == 0)
                    return NotFound(ApiResponse<object>.Fail("Không có dữ liệu chuyên ngành nào."));

                return Ok(ApiResponse<List<MajorDTO>>.Success(majors, "Lấy danh sách chuyên ngành thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }

        [HttpGet("getAllCodeCourse")]
        public async Task<IActionResult> GetAllMajorsCategories()
        {
            try
            {
                var majors = await _majorService.GetAllCoursesAsync();

                if (majors == null || majors.Count == 0)
                    return NotFound(ApiResponse<object>.Fail("Không có dữ liệu chuyên ngành nào."));

                return Ok(ApiResponse<List<MajorCategoryDTO>>.Success(majors, "Lấy danh sách môn học thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }
    }
}
