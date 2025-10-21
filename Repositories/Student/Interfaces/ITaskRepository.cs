using DataTranferObjects.Staff.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Student.Interfaces
{
    public interface ITaskRepository
    {
        Task<Entities.Models.Task> CreateTaskAsync(Entities.Models.Task task, int assignedUserId, int createdBy, int? reviewerId = null);
        Task<List<TaskDto>> GetTasksByGroupIdAsync(int groupId);
        Task<TaskDto?> GetTaskByIdAsync(int taskId);
        Task<Entities.Models.Task?> UpdateTaskAsync(UpdateTaskDTO dto, int updatedBy);
    }
}
