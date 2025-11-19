using DataTranferObjects.Staff.Semester;
using DataTranferObjects.Staff.Task;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Student.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<Entities.Models.Task>> GetAllActiveMeetingTasksAsync(int groupId);
        Task<Entities.Models.Task> CreateTaskAsync(Entities.Models.Task task, int assignedUserId, int createdBy, int? reviewerId = null);
        Task<List<TaskDto>> GetTasksByGroupIdAsync(int groupId);
        Task<TaskDto?> GetTaskByIdAsync(int taskId);
        Task<List<Entities.Models.Task>> GetTasksByAssigneeAsync(int userId);
        Task<Entities.Models.Task?> UpdateTaskAsync(UpdateTaskDTO dto, int updatedBy);
        Task<object?> GetMeetingScheduleWithTasksAsync(int meetingScheduleId);
        Task<List<Entities.Models.Task>> GetTasksByReviewerAsync(int userId, int groupId);
    }
}
