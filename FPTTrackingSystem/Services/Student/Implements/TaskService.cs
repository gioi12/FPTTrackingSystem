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
            var newTask = new Entities.Models.Task
            {
                GroupId = dto.GroupId,
                Name = dto.Name,
                PriorityId = dto.PriorityId,
                Process = dto.Process,
                Description = dto.Description,
                Deadline = dto.EndAt,
                StatusId = dto.StatusId,
                MilestoneId = dto.MilestoneId
            };

            return await _taskRepository.CreateTaskAsync(newTask, dto.AssignedUserId, user.Id ?? 0);
        }

        public async Task<ApiResponse<List<TaskDto>>> GetTasksByGroupIdAsync(int groupId)
        {
            var tasks = await _taskRepository.GetTasksByGroupIdAsync(groupId);

            if (tasks == null || !tasks.Any())
            {
                return new ApiResponse<List<TaskDto>>
                {
                    Status = 404,
                    Message = "Không tìm thấy task nào trong nhóm này.",
                    Data = null
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
                    Status = 404,
                    Message = "Không tìm thấy task với ID này.",
                    Data = null
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
                // Lấy user hiện tại từ cookie
                var user = await _authUtils.GetUserInfoFromCookie();

                // Gọi repository để update task
                var updatedTask = await _taskRepository.UpdateTaskAsync(dto, user.Id ?? 0);

                if (updatedTask == null)
                    return new ApiResponse<TaskResponseUpdateDto>(404, "Không tìm thấy task");

                var assignedUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.AssignedUserId);

                var responseDto = new TaskResponseUpdateDto
                {
                    Id = updatedTask.Id,
                    Name = updatedTask.Name,
                    Description = updatedTask.Description,
                    Deadline = updatedTask.Deadline,
                    StatusId = updatedTask.StatusId,
                    PriorityId = updatedTask.PriorityId,
                    Process = updatedTask.Process,
                    MilestoneId = updatedTask.MilestoneId,
                    GroupId = updatedTask.GroupId,
                    AssignedUserId = dto.AssignedUserId,
                    AssignedUserName = assignedUser?.Fullname
                };

                return new ApiResponse<TaskResponseUpdateDto>(200, "Cập nhật thành công", responseDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateTaskAsync] Error: {ex.Message}");
                return new ApiResponse<TaskResponseUpdateDto>(500, "Lỗi khi cập nhật task: " + ex.Message);
            }
        }

    }
}
