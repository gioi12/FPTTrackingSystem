using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Student.Interfaces
{
    public interface ITaskService
    {
        Task<Entities.Models.Task> CreateTaskAsync(CreateTaskDTO dto);
        Task<ApiResponse<List<TaskDto>>> GetTasksByGroupIdAsync(int groupId);
        Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int taskId);
        Task<ApiResponse<TaskResponseUpdateDto>> UpdateTaskAsync(UpdateTaskDTO dto);
        Task<object?> GetMeetingScheduleWithTasksAsync(int meetingScheduleId);
        Task<List<TaskDto>> GetTasksByAssigneeAsync();
    }
}
