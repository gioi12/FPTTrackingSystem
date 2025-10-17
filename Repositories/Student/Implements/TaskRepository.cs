using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Task;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Student.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositories.Student.Implements
{
    public class TaskRepository : ITaskRepository
    {
        private readonly FpttrackingSystemContext _context;
        private readonly ILogger<TaskRepository> _logger;
        public TaskRepository(FpttrackingSystemContext context, ILogger<TaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Entities.Models.Task> CreateTaskAsync(Entities.Models.Task task, int assignedUserId, int createdBy)
        {
            try
            {
                // Thêm task mới
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                var creator = new TaskUser
                {
                    TaskId = task.Id,
                    UserId = createdBy,
                    IsCreated = true
                };

                var assignee = new TaskUser
                {
                    TaskId = task.Id,
                    UserId = assignedUserId,
                    IsCreated = false
                };

                await _context.TaskUsers.AddRangeAsync(creator, assignee);
                await _context.SaveChangesAsync();

                var log = new Log
                {
                    Name = "Create Task",
                    EntityName = "task",
                    EntityId = task.Id,
                    Action = "CREATE",
                    Description = $"Người dùng ID {createdBy} đã tạo task \"{task.Name}\" và giao cho user ID {assignedUserId}.",
                    UserId = createdBy,
                    CreateAt = DateTime.Now
                };

                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating task: {TaskName}", task?.Name);
                throw; 
            }
        }


        public async Task<List<TaskDto>> GetTasksByGroupIdAsync(int groupId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Group)
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
                    .Where(t => t.GroupId == groupId)
                    .OrderBy(t => t.Deadline)
                    .ToListAsync();

                var result = tasks.Select(task =>
                {
                    var createdByUser = task.TaskUsers.FirstOrDefault(tu => tu.IsCreated);
                    var assignee = task.TaskUsers.FirstOrDefault(tu => !tu.IsCreated);

                    return new TaskDto
                    {
                        Id = task.Id,
                        Title = task.Name,
                        Description = task.Description,
                        Deadline = task.Deadline,
                        CreatedAt = task.CreatedAt,
                        CreatedBy = createdByUser?.User.Id,
                        CreatedByName = createdByUser?.User.Fullname,
                        Priority = task.Priority,
                        Status = task.Status,
                        Process = task.Process,
                        AssigneeId = assignee?.User.Id,
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
                        Attachments = _context.Attachments?
                            .Where(a => a.EntityName.Equals("task") && a.EntityId == task.Id)
                            .Select(a => new AttachmentDto
                            {
                                Id = a.Id,
                                FileName = a.FileName,
                                FileUrl = a.FilePath
                            }).ToList() ?? new List<AttachmentDto>(),
                        Comments = _context.Comments?
                            .Where(c => c.EntityName.Equals("task") && c.EntityId == task.Id)
                            .Select(c => new CommentDto
                            {
                                Id = c.Id,
                                Author = c.User.RollNumber ?? "",
                                AuthorName = c.User.Fullname,
                                Content = c.Feedback ?? "",
                                Timestamp = c.CreateAt
                            }).ToList() ?? new List<CommentDto>(),
                        History = _context.Logs
                            .Where(h => h.EntityName.Equals("task") && h.EntityId == task.Id)
                            .Select(h => new HistoryDto
                            {
                                Id = h.Id,
                                Detail = h.Description,
                                At = h.CreateAt,
                                User = h.User.RollNumber,
                                Action = h.Action
                            }).ToList()
                    };
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetTasksByGroupIdAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new List<TaskDto>();
            }
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
                    Priority = task.Priority,
                    Status = task.Status,
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
                    Attachments = _context.Attachments?.Where(m => m.EntityName.Equals("task") && m.EntityId == taskId).Select(a => new AttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileUrl = a.FilePath
                    }).ToList() ?? new List<AttachmentDto>(),
                    Comments = _context.Comments?.Where(m => m.EntityName.Equals("task") && m.EntityId == taskId).Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Author = c.User.RollNumber ?? "",
                        AuthorName = c.User.Fullname,
                        Content = c.Feedback ?? "",
                        Timestamp = c.CreateAt
                    }).ToList() ?? new List<CommentDto>(),

                    History = _context.Logs.Where(m => m.EntityName.Equals("task") && m.EntityId == taskId).Select(h => new HistoryDto
                    {
                        Id = h.Id,
                        Detail = h.Description,
                        At = h.CreateAt,
                        User = h.User.RollNumber,
                        Action = h.Action
                    }).ToList()
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

        public async Task<Entities.Models.Task?> UpdateTaskAsync(UpdateTaskDTO dto, int updatedBy)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskUsers)
                .FirstOrDefaultAsync(t => t.Id == dto.Id);

            if (task == null)
                return null;

            task.Name = dto.Name;
            task.Description = dto.Description;
            task.Deadline = dto.EndAt;
            task.Status = dto.StatusId;
            task.Priority = dto.PriorityId;
            task.Process = dto.Process;
            task.MilestoneId = dto.MilestoneId;

            var assignedUser = task.TaskUsers.FirstOrDefault(tu => !tu.IsCreated);

            if (assignedUser != null)
            {
                if (assignedUser.UserId != dto.AssignedUserId)
                    assignedUser.UserId = dto.AssignedUserId;
            }
            else
            {
                var newTaskUser = new TaskUser
                {
                    TaskId = task.Id,
                    UserId = dto.AssignedUserId,
                    IsCreated = false
                };
                _context.TaskUsers.Add(newTaskUser);
            }

            var createdUser = task.TaskUsers.FirstOrDefault(tu => tu.IsCreated);
            if (createdUser == null)
            {
                var newCreator = new TaskUser
                {
                    TaskId = task.Id,
                    UserId = updatedBy,
                    IsCreated = true
                };
                _context.TaskUsers.Add(newCreator);
            }

            await _context.SaveChangesAsync();

            var log = new Log
            {
                Name = $"Cập nhật Task: {task.Name}",
                EntityName = "Task",
                EntityId = task.Id,
                Action = "UPDATE",
                Description = $"Người dùng ID {updatedBy} đã cập nhật task '{task.Name}'",
                UserId = updatedBy,
                CreateAt = DateTime.Now
            };

            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();

            return task;
        }




    }
}
