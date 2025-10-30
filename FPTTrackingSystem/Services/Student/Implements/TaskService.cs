using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Task;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.EntityFrameworkCore;
using Repositories.Student.Interfaces;

namespace FPTTrackingSystem.Services.Student.Implements
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly AuthUtils _authUtils;
        private readonly FpttrackingSystemContext _context;

        public TaskService(ITaskRepository taskRepository, AuthUtils authUtils, FpttrackingSystemContext context)
        {
            _taskRepository = taskRepository;
            _authUtils = authUtils;
            _context = context;
        }

        public async Task<Entities.Models.Task> CreateTaskAsync(CreateTaskDTO dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role == "Student" && (user.Groups == null || !user.Groups.Contains(dto.GroupId)))
                throw new UnauthorizedAccessException("Bạn không có quyền tạo task trong nhóm này.");

            if (dto.GroupId <= 0)
                throw new ArgumentException("GroupId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Task name không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.TaskType))
                throw new ArgumentException("TaskType không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Priority))
                throw new ArgumentException("Priority không được để trống.");

            if (dto.EndAt == default)
                throw new ArgumentException("EndAt không được để trống.");

            if (dto.AssignedUserId <= 0)
                throw new ArgumentException("AssignedUserId không hợp lệ.");

            if (dto.ReviewerId <= 0)
                throw new ArgumentException("ReviewerId không hợp lệ.");

            var validTaskTypes = new[] { "todo", "progress", "done" };
            if (string.IsNullOrWhiteSpace(dto.TaskType) ||
                !validTaskTypes.Contains(dto.TaskType.Trim().ToLower()))
                throw new ArgumentException("Invalid TaskType. Allowed values: ToDo, Progress, Done.");

            var validPriorities = new[] { "high", "medium", "low" };
            if (string.IsNullOrWhiteSpace(dto.Priority) ||
                !validPriorities.Contains(dto.Priority.Trim().ToLower()))
                throw new ArgumentException("Invalid Priority. Allowed values: High, Medium, Low.");

            var formattedTaskType = char.ToUpper(dto.TaskType[0]) + dto.TaskType.Substring(1).ToLower();
            var formattedPriority = char.ToUpper(dto.Priority[0]) + dto.Priority.Substring(1).ToLower();
            var newTask = new Entities.Models.Task
            {
                GroupId = dto.GroupId,
                Name = dto.Name,
                Priority = formattedPriority,
                Process = dto.Process,
                Description = dto.Description,
                Deadline = dto.EndAt,
                Status = dto.Status,
                DeliverableId = dto.DeliverableId,
                Type = formattedTaskType,
                CreatedAt = DateTime.Now,
                IsActive = true,
                MeetingScheduleDateId = dto.MeetingId > 0 ? dto.MeetingId : null
            };

            return await _taskRepository.CreateTaskAsync(
                newTask,
                dto.AssignedUserId,
                user.Id ?? 0,
                dto.ReviewerId
            );
        }

        public async Task<ApiResponse<List<TaskDto>>> GetTasksByGroupIdAsync(int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role == "Student" || user.Role == "Supervisor")
            {
                if (user.Groups == null || !user.Groups.Contains(groupId))
                {
                    return new ApiResponse<List<TaskDto>>(403, "Bạn không có quyền xem task của nhóm này.", null);
                }
            }

            var tasks = await _taskRepository.GetTasksByGroupIdAsync(groupId);

            if (tasks == null || !tasks.Any())
            {
                return new ApiResponse<List<TaskDto>>(200, "Không tìm thấy task nào trong nhóm này.", new List<TaskDto>());
            }

            return new ApiResponse<List<TaskDto>>(200, "Lấy danh sách task thành công.", tasks);
        }

        public async Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int taskId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var task = await _taskRepository.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                return new ApiResponse<TaskDto>(200, "Không tìm thấy task với ID này.", new TaskDto());
            }

            if ((user.Role == "Student" || user.Role == "Supervisor") &&
                (user.Groups == null || !user.Groups.Contains(task.Group.Id)))
            {
                return new ApiResponse<TaskDto>(403, "Bạn không có quyền xem task này.", null);
            }

            return new ApiResponse<TaskDto>(200, "Lấy thông tin task thành công.", task);
        }

        public async Task<ApiResponse<TaskResponseUpdateDto>> UpdateTaskAsync(UpdateTaskDTO dto)
        {
            try
            {

                var user = await _authUtils.GetUserInfoFromCookie();
                var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == dto.Id);
                var taskCreator = await _context.TaskUsers
                        .FirstOrDefaultAsync(tu => tu.TaskId == dto.Id && tu.Type == "Creator");
                if (existingTask == null)
                    return new ApiResponse<TaskResponseUpdateDto>(200, "Không tìm thấy task", null);

                if (user.Role == "Student")
                {
                    if (user.Groups == null || !user.Groups.Contains(existingTask.GroupId))
                        return new ApiResponse<TaskResponseUpdateDto>(403, "Bạn không có quyền sửa task của nhóm khác.", null);

                    if (taskCreator == null || taskCreator.UserId != user.Id)
                        return new ApiResponse<TaskResponseUpdateDto>(403, "Bạn chỉ được sửa task do chính mình tạo.", null);
                }

                if (dto.GroupId <= 0)
                    throw new ArgumentException("GroupId không hợp lệ.");

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("Task name không được để trống.");

                if (string.IsNullOrWhiteSpace(dto.StatusId))
                    throw new ArgumentException("TaskType không được để trống.");

                if (string.IsNullOrWhiteSpace(dto.PriorityId))
                    throw new ArgumentException("Priority không được để trống.");

                if (dto.EndAt == default)
                    throw new ArgumentException("EndAt không được để trống.");

                if (dto.AssignedUserId <= 0)
                    throw new ArgumentException("AssignedUserId không hợp lệ.");

                if (dto.ReviewerId <= 0)
                    throw new ArgumentException("ReviewerId không hợp lệ.");
                var validTaskTypes = new[] { "todo", "progress", "done" };
                if (string.IsNullOrWhiteSpace(dto.StatusId) ||
                    !validTaskTypes.Contains(dto.StatusId.Trim().ToLower()))
                    throw new ArgumentException("Invalid TaskType. Allowed values: ToDo, Progress, Done.");

                var validPriorities = new[] { "high", "medium", "low" };
                if (!validPriorities.Contains(dto.PriorityId))
                    return ApiResponse<TaskResponseUpdateDto>.Fail("Độ ưu tiên không hợp lệ.", 400);
                var updatedTask = await _taskRepository.UpdateTaskAsync(dto, user.Id ?? 0);

                var assignedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.AssignedUserId);
                var reviewerUser = dto.ReviewerId != null
                    ? await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.ReviewerId)
                    : null;

                var responseDto = new TaskResponseUpdateDto
                {
                    Id = updatedTask.Id,
                    Name = updatedTask.Name,
                    Description = updatedTask.Description,
                    Deadline = updatedTask.Deadline,
                    StatusId = updatedTask.Status,
                    PriorityId = updatedTask.Priority,
                    Process = updatedTask.Process,
                    MilestoneId = updatedTask.DeliverableId,
                    GroupId = updatedTask.GroupId,
                    AssignedUserId = dto.AssignedUserId,
                    AssignedUserName = assignedUser?.Fullname,
                    ReviewerId = dto.ReviewerId,
                    ReviewerName = reviewerUser?.Fullname,
                    MeetingId = dto.MeetingId
                };

                return new ApiResponse<TaskResponseUpdateDto>(200, "Cập nhật thành công", responseDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateTaskAsync] Error: {ex.Message}");
                return new ApiResponse<TaskResponseUpdateDto>(500, "Lỗi khi cập nhật task: " + ex.Message);
            }
        }

        public async Task<object?> GetMeetingScheduleWithTasksAsync(int meetingScheduleId)
        {
            return await _taskRepository.GetMeetingScheduleWithTasksAsync(meetingScheduleId);
        }
    }
}
