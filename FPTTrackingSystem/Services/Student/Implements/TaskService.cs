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

            var priority = string.IsNullOrWhiteSpace(dto.Priority)
                ? string.Empty
                : char.ToUpper(dto.Priority[0]) + dto.Priority.Substring(1).ToLower();

            // Khởi tạo task mới
            var newTask = new Entities.Models.Task
            {
                GroupId = dto.GroupId,
                Name = dto.Name,
                Priority = priority,
                Process = dto.Process,
                Description = dto.Description,
                Deadline = dto.EndAt,
                Status = dto.Status,
                DeliverableId = dto.DeliverableId,
                Type = dto.TaskType, 
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
            var tasks = await _taskRepository.GetTasksByGroupIdAsync(groupId);

            if (tasks == null || !tasks.Any())
            {
                return new ApiResponse<List<TaskDto>>
                {
                    Status = 200,
                    Message = "Không tìm thấy task nào trong nhóm này.",
                    Data = new List<TaskDto>() 
                };
            }

            return new ApiResponse<List<TaskDto>>
            {
                Status = 200,
                Message = "Lấy danh sách task thành công.",
                Data = tasks
            };
        }

        public async Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                return new ApiResponse<TaskDto>
                {
                    Status = 200,
                    Message = "Không tìm thấy task với ID này.",
                    Data = new TaskDto() 
                };
            }

            return new ApiResponse<TaskDto>
            {
                Status = 200,
                Message = "Lấy thông tin task thành công.",
                Data = task
            };
        }

        public async Task<ApiResponse<TaskResponseUpdateDto>> UpdateTaskAsync(UpdateTaskDTO dto)
        {
            try
            {
                var user = await _authUtils.GetUserInfoFromCookie();

                var updatedTask = await _taskRepository.UpdateTaskAsync(dto, user.Id ?? 0);

                if (updatedTask == null)
                    return new ApiResponse<TaskResponseUpdateDto>(200, "Không tìm thấy task", null);
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
