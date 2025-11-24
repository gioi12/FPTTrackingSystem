using DataTranferObjects.Staff.Task;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTTrackingSystem.Controllers.Student
{
    [Route("api/")]
    public class TaskController : Controller
    {
        private readonly AuthUtils _authUtils;
        private readonly ITaskService _taskService;
        private readonly FpttrackingSystemContext _context;
        public TaskController(ITaskService taskService, AuthUtils authUtils, FpttrackingSystemContext context)
        {
            _taskService = taskService;
            _context = context;
            _authUtils = authUtils;
        }

        [HttpGet("v1/Student/Task/Incomplete/{groupId}")]
        public async Task<IActionResult> GetAllActiveMeetingTasks(int groupId)
        {
            try
            {
                var tasks = await _taskService.GetAllActiveMeetingTasksAsync(groupId);
                return Ok(ApiResponse<List<TaskResponsesDto>>.Success(tasks, "Retrieved meeting tasks successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpPost("v1/Student/Task/create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(dto);

                if (createdTask == null)
                    return BadRequest(ApiResponse<object>.Fail("Tạo task thất bại.", 400));

                // Sau khi task được tạo, truy vấn danh sách TaskUser để lấy role thực tế
                var taskUsers = await _context.TaskUsers
                    .Where(tu => tu.TaskId == createdTask.Id)
                    .ToListAsync();

                var creatorId = taskUsers.FirstOrDefault(tu => tu.Type == "Creator")?.UserId;
                var assigneeId = taskUsers.FirstOrDefault(tu => tu.Type == "Assignee")?.UserId;
                var reviewerId = taskUsers.FirstOrDefault(tu => tu.Type == "Reviewer")?.UserId;

                return Ok(ApiResponse<object>.Success(new
                {
                    createdTask.Id,
                    createdTask.GroupId,
                    createdTask.Name,
                    createdTask.Description,
                    createdTask.Deadline,
                    CreatorId = creatorId,
                    AssignedUserId = assigneeId,
                    ReviewerId = reviewerId,
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

        [HttpGet("v1/Student/Task/get-by-group/{groupId}")]
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

        [HttpGet("v1/Tasks/reviewer")]
        public async Task<IActionResult> GetTasksByReviewer([FromQuery] int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
            {
                return Unauthorized(new { message = "User not logged in." });
            }

            var tasks = await _taskService.GetReviewerTasksAsync(user.Id ?? 0, groupId);

            if (tasks == null || !tasks.Any())
            {
                return NotFound(new { message = "No reviewer tasks found in this group." });
            }

            return Ok(tasks);
        }


        [HttpGet("v1/task/taskTypeIssue/{groupId}")]
        public async Task<IActionResult> GetTaskTypeIssue(int groupId)
        {
            var result = await _taskService.GetTaskTypeIssueByGroupIdAsync(groupId);

            return Ok(new
            {
                Code = 200,
                Message = "Danh sách task type = 'issue'",
                Data = result
            });
        }

        [HttpGet("v1/task/statistic/assignee")]
        public async Task<IActionResult> GetTaskStatisticByAssignee()
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                return Unauthorized("User not logged in");

            var result = await _taskService.GetTaskStatisticByAssigneeAsync(user.Id ?? 0);

            return Ok(result);
        }

        [HttpGet("v1/Student/Task/assignee")]
        public async Task<IActionResult> GetTasksByAssignee()
        {
            var tasks = await _taskService.GetTasksByAssigneeAsync();

            return Ok(new
            {
                statusCode = 200,
                message = tasks != null && tasks.Any()
                    ? "Success"
                    : "No tasks found for this assignee.",
                data = tasks ?? new List<TaskDto>()
            });
        }


        [HttpGet("v1/Student/Task/get-by-id/{taskId}")]
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

        [HttpPost("v1/Student/Task/update")]
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

        [HttpGet("v1/Student/Task/meeting-tasks/{meetingScheduleId}")]
        public async Task<IActionResult> GetTasksByMeetingScheduleId(int meetingScheduleId)
        {
            var tasks = await _taskService.GetMeetingScheduleWithTasksAsync(meetingScheduleId);

            if (tasks == null)
            {
                return Ok(new { success = 200, data = new List<object>(), message = "No tasks found for this meeting schedule." });
            }

            return Ok(new { success = 200, data = tasks });
        }

        [Authorize]
        [HttpPost("v1/upload/task")]
        public async Task<object> UploadMTask(IFormFile file, int groupId,int taskId)
        {
            var message = await _taskService.UploadFileTask(file, groupId,taskId);
            return Ok(ApiResponse<object>.Success(message, "Upload Successfully"));
        }
        [Authorize]
        [HttpDelete("v1/upload/task")]
        public async Task<object> DeleteTask(int attachmentId)
        {
            await _taskService.DeleteFileTask(attachmentId);
            return Ok(ApiResponse<object>.Success(null, "Delete attachment successfully."));
        }
        [Authorize]
        [HttpGet("v1/upload/tasks")]
        public async Task<object> FilesGroup(int groupId,int taskId)
        {
            var list = await _taskService.GetFilesTask(groupId, taskId);
            return Ok(ApiResponse<object>.Success(list, "Get attachments successfully."));
        }
    }
}
