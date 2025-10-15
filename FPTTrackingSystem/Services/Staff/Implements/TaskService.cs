using DataTranferObjects.Staff.Task;
using FPTTrackingSystem.Services.Staff.Interfaces;
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
                CreateAt = DateTime.Now,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                StatusId = 1,
            };

            return await _taskRepository.CreateTaskAsync(newTask, dto.AssignedUserId);
        }
    }
}
