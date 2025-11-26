using DataTranferObjects.Common.Response;
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
        Task<List<TaskResponsesDto>> GetAllActiveMeetingTasksAsync(int groupId);
        Task<string> UploadFileTask(IFormFile file, int groupId,int taskId, string semester);
        Task DeleteFileTask(int attachmentId);
        Task<List<AttachmentRes>> GetFilesTask(int groupId, int taskId);
        Task<List<TaskReviewerDTO>> GetReviewerTasksAsync(int userId, int groupId);
        Task<TaskStatisticResponse> GetTaskStatisticByAssigneeAsync(int userId);
        Task<List<TaskReviewerDTO>> GetTaskTypeIssueByGroupIdAsync(int groupId);
        Task<ApiResponse<string>> DeleteTaskAsync(int taskId);
    }
}
