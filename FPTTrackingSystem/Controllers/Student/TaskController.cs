using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Student
{
    [Route("api/v1/Student/Task")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(dto);

                if (createdTask == null)
                    return BadRequest(ApiResponse<object>.Fail("Tạo task thất bại."));

                return Ok(ApiResponse<object>.Success(new
                {
                    createdTask.GroupId,
                    createdTask.Name,
                    createdTask.Description,
                    createdTask.Deadline,
                    AssignedUserId = dto.AssignedUserId
                }, "Tạo task thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }
    }
}
