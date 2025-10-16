using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Repositories.Staff.Interfaces;

namespace FPTTrackingSystem.Services.Staff.Implements
{
    public class TaskService: ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<Entities.Models.Task> CreateTaskAsync(CreateTaskDTO dto)
        {
            var newTask = new Entities.Models.Task
            {
                GroupId = dto.GroupId,
                Name = dto.Name,
                Description = dto.Description,
                Deadline = dto.EndAt,
                StatusId =(int)StatusTask.ToDo,
            };

            return await _taskRepository.CreateTaskAsync(newTask, dto.AssignedUserId);
        }

        public async Task<ApiResponse<List<TaskResponseDto>>> GetTasksByGroupIdAsync(int groupId)
        {
            var tasks = await _taskRepository.GetTasksByGroupIdAsync(groupId);

            if (tasks == null || !tasks.Any())
            {
                return new ApiResponse<List<TaskResponseDto>>
                {
                    Status = 404,
                    Message = "Không tìm thấy task nào trong nhóm này.",
                    Data = null
                };
            }

            return new ApiResponse<List<TaskResponseDto>>
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


    }
}
