using DataTranferObjects.Staff.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Interfaces
{
    public interface ITaskRepository
    {
        Task<Entities.Models.Task> CreateTaskAsync(Entities.Models.Task task, int assignedUserId);
        Task<List<TaskResponseDto>> GetTasksByGroupIdAsync(int groupId);
        Task<TaskDto?> GetTaskByIdAsync(int taskId);
    }
}
