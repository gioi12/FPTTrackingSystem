using DataTranferObjects.Staff.Task;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface ITaskService
    {
        Task<Entities.Models.Task> CreateTaskAsync(CreateTaskDTO dto);
    }
}
