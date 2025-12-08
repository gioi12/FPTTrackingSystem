using DataTranferObjects.Staff.Group;
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

        /*        [HttpGet("GetMajors")]  
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
                }*/
        [HttpGet("getAllCodeCourseV2")]
        public async Task<IActionResult> GetAllMajorsCategories(int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _majorService.GetAllCoursesPagedAsync(page, pageSize);

                if (result.Items == null)
                    return NotFound(ApiResponse<object>.Fail("Không có dữ liệu môn học."));

                return Ok(ApiResponse<PagedData<MajorCategoryDTO>>.Success(result,
                    "Lấy danh sách môn học thành công."));
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


        [HttpGet("GetCourseBy/{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            try
            {
                var result = await _majorService.GetByIdAsync(id);
                if (result == null)
                    return Ok(ApiResponse<object>.Success(null,"Không tìm thấy môn học."));

                return Ok(ApiResponse<MajorCategoryDTO>.Success(result, "Lấy môn học thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _majorService.GetByIdAsync(id);
            if (result == null)
                return Ok(ApiResponse<object>.Success(null,"Không tìm thấy bản ghi."));
            return Ok(ApiResponse<object>.Success(result, "Lấy dữ liệu thành công."));
        }

        [HttpPost("createCourse")]
        public async Task<IActionResult> Create([FromBody] MajorCategoryDTO dto)
        {
            if (dto.Size == null || dto.Size <= 0)
                return BadRequest(ApiResponse<object>.Fail("Size must be greater than 0."));

            var success = await _majorService.CreateAsync(dto);
            if (!success)
                return BadRequest(ApiResponse<object>.Fail("Tạo thất bại."));
            return Ok(ApiResponse<object>.Success(null, "Tạo thành công."));
        }

        [HttpPost("updateCourse/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MajorCategoryDTO dto)
        {
            dto.Id = id;
            if (dto.Size == null || dto.Size <= 0)
                return BadRequest(ApiResponse<object>.Fail("Size must be greater than 0."));

            var success = await _majorService.UpdateAsync(dto);
            if (!success)
                return Ok(ApiResponse<object>.Success(null,"Không tìm thấy bản ghi để cập nhật."));
            return Ok(ApiResponse<object>.Success(null, "Cập nhật thành công."));
        }
    }
}
