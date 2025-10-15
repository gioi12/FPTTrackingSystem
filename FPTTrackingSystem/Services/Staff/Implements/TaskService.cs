using DataTranferObjects.Enum;
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
                Deadline = dto.EndAt,
                StatusId =(int)StatusTask.ToDo,
            };

            return await _taskRepository.CreateTaskAsync(newTask, dto.AssignedUserId);
        }
    }
}
