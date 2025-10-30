using DataTranferObjects.Staff.Task;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
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
                    AssignedUserId = dto.AssignedUserId,
                    ReviewerId = dto.ReviewerId
                }, "Tạo task thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }

        [HttpGet("get-by-group/{groupId}")]
        public async Task<IActionResult> GetTasksByGroup(int groupId)
        {
            try
            {
                var response = await _taskService.GetTasksByGroupIdAsync(groupId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)

            { 
                return StatusCode(500, new ApiResponse<string>
                {
                    Status = 500,
                    Message = "Đã xảy ra lỗi trong quá trình xử lý.",
                    Data = null
                });
            }
        }

        [HttpGet("get-by-id/{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            try
            {
                var response = await _taskService.GetTaskByIdAsync(taskId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail("Đã xảy ra lỗi khi xử lý yêu cầu."));
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDTO dto)
        {
            try
            {
                var response = await _taskService.UpdateTaskAsync(dto);
                return StatusCode(response.Status, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }

        [HttpGet("meeting-tasks/{meetingScheduleId}")]
        public async Task<IActionResult> GetTasksByMeetingScheduleId(int meetingScheduleId)
        {
            var tasks = await _taskService.GetMeetingScheduleWithTasksAsync(meetingScheduleId);

            if (tasks == null)
            {
                return Ok(new { success = 200, data = new List<object>(), message = "No tasks found for this meeting schedule." });
            }

            return Ok(new { success = 200, data = tasks });
        }
    }
}
