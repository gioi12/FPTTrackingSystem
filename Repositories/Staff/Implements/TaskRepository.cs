using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositories.Staff.Implements
{
    public class TaskRepository: ITaskRepository
    {
        private readonly FpttrackingSystemContext _context;

        public TaskRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<Entities.Models.Task> CreateTaskAsync(Entities.Models.Task task, int assignedUserId)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            var taskUser = new TaskUser
            {
                TaskId = task.Id,
                UserId = assignedUserId,
                IsCreated = true
            };

            _context.TaskUsers.Add(taskUser);
            await _context.SaveChangesAsync();

            return task;
        }
    }
}
