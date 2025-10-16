using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Task;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TaskRepository> _logger;
        public TaskRepository(FpttrackingSystemContext context, ILogger<TaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Entities.Models.Task> CreateTaskAsync(Entities.Models.Task task, int assignedUserId)
        {
            try
            {
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating task: {TaskName}", task?.Name);
            }

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

        public async Task<List<TaskResponseDto>> GetTasksByGroupIdAsync(int groupId)
        {
            return await _context.Tasks
                .Where(t => t.GroupId == groupId)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    EndAt = t.Deadline,
                    GroupId = t.GroupId,

                    AssignedTo = t.TaskUsers.FirstOrDefault() != null ? t.TaskUsers.FirstOrDefault().UserId : null,
                    AssignedToName = t.TaskUsers.FirstOrDefault() != null ? t.TaskUsers.FirstOrDefault().User.Fullname : null
                })
                .OrderBy(t => t.EndAt)
                .ToListAsync();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId)
        {
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.Group)
                    .Include(t => t.Status)
                    .Include(t => t.Priority)
                    .Include(t => t.Milestone)
                    .Include(t => t.TaskUsers)
                        .ThenInclude(tu => tu.User)
                            .ThenInclude(u => u.Attachments)
                    .Include(t => t.TaskUsers)
                        .ThenInclude(tu => tu.User)
                            .ThenInclude(u => u.Comments)
                    .Include(t => t.TaskUsers)
                        .ThenInclude(tu => tu.User)
                            .ThenInclude(u => u.Logs)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    Console.WriteLine($"[GetTaskByIdAsync] Task with ID={taskId} not found.");
                    return null;
                }

                var createdByUser = task.TaskUsers.FirstOrDefault(tu => tu.IsCreated);
                var assignee = task.TaskUsers.FirstOrDefault(tu => !tu.IsCreated);


                var dto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Name,
                    Description = task.Description,
                    Deadline = task.Deadline,
                    CreatedAt = task.CreatedAt,
                    CreatedBy = createdByUser?.Id,
                    CreatedByName = createdByUser?.User.Fullname,
                    Priority = task.Priority.Name,
                    Status = task.Status.Name,
                    Process = task.Process,
                    AssigneeId = assignee?.Id,
                    AssigneeName = assignee?.User.Fullname,
                    Group = task.Group != null
                        ? new GroupTaskDto { Id = task.Group.Id, Name = task.Group.Name }
                        : null,
                    Milestone = task.Milestone != null
                            ? new MilestonesDto
                            {
                                Id = task.Milestone.Id,
                                Name = task.Milestone.Name,
                                StartAt = task.Milestone.CreateAt,
                                Description = task.Milestone.Description
                            }
                            : null,
                    Attachments = assignee?.User.Attachments?.Select(a => new AttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileUrl = a.FilePath
                    }).ToList() ?? new List<AttachmentDto>(),
                    Comments = assignee?.User.Comments?.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Author = c.User?.RollNumber ?? "",
                        AuthorName = c.User?.Fullname ?? "",
                        Content = c.Feedback ?? "",
                        Timestamp = c.CreateAt
                    }).ToList() ?? new List<CommentDto>(),

                    // History = ...
                };

                return dto;
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết
                Console.WriteLine($"[GetTaskByIdAsync] Error while processing TaskId={taskId}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Nếu bạn có LogRepository, dùng như này:
                // _logRepository.CreateLog(new Log { Type = "Error", Detail = ex.ToString(), CreateAt = DateTime.Now });

                // Tránh throw ra ngoài, trả null để API bắt lỗi
                return null;
            }
        }



    }
}
