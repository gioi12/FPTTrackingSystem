using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface ITaskService
    {
        Task<Entities.Models.Task> CreateTaskAsync(CreateTaskDTO dto);
        Task<ApiResponse<List<TaskResponseDto>>> GetTasksByGroupIdAsync(int groupId);
        Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int taskId);
    }
}
