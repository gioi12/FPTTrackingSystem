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

        public async Task<Entities.Models.Task> CreateTaskAsync(Entities.Models.Task task, int assignedUserId,int createdBy,int? reviewerId = null)
        {
            try
            {
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();

                var creator = new TaskUser
                {
                    TaskId = task.Id,
                    UserId = createdBy,
                    Type = "Creator"
                };

                var assignee = new TaskUser
                {
                    TaskId = task.Id,
                    UserId = assignedUserId,
                    Type = "Assignee"
                };

                var taskUsers = new List<TaskUser> { creator, assignee };
                if (reviewerId.HasValue && reviewerId.Value > 0)
                {
                    taskUsers.Add(new TaskUser
                    {
                        TaskId = task.Id,
                        UserId = reviewerId.Value,
                        Type = "Reviewer"
                    });
                }

                await _context.TaskUsers.AddRangeAsync(taskUsers);
                await _context.SaveChangesAsync();

                var log = new Log
                {
                    Name = "Create Task",
                    EntityName = "task",
                    EntityId = task.Id,
                    Action = "CREATE",
                    Description = $"Người dùng ID {createdBy} đã tạo task \"{task.Name}\" " +
              $"và giao cho user ID {assignedUserId}" +
              $"{(reviewerId.HasValue && reviewerId.Value > 0 ? $" (reviewer ID {reviewerId.Value})" : "")}.",
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

        public async Task<List<Entities.Models.Task>> GetTasksByAssigneeAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.Group)
                .Include(t => t.Deliverable)
                .Include(t => t.Comments)
                .Include(t => t.TaskUsers)
                    .ThenInclude(tu => tu.User)
                .Where(t => t.TaskUsers.Any(tu => tu.UserId == userId && tu.Type == "Assignee"))
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }

        public async Task<List<TaskDto>> GetTasksByGroupIdAsync(int groupId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Group)
                    .Include(t => t.Deliverable)
                    .Include(t => t.Comments)
                    .Include(t => t.TaskUsers)
                        .ThenInclude(tu => tu.User)
                    .Where(t => t.GroupId == groupId)
                    .OrderBy(t => t.Deadline)
                    .ToListAsync();

                var result = tasks.Select(task =>
                {
                    var createdByUser = task.TaskUsers?.FirstOrDefault(tu => tu.Type == "Creator");
                    var assignee = task.TaskUsers?.FirstOrDefault(tu => tu.Type == "Assignee");
                    var reviewer = task.TaskUsers?.FirstOrDefault(tu => tu.Type == "Reviewer");

                    bool isMeetingTask = task.MeetingScheduleDateId.HasValue;
                    int meetingId = isMeetingTask ? task.MeetingScheduleDateId.Value : 0;

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
                        AssigneeId = assignee?.User.Id,
                        AssigneeName = assignee?.User.Fullname,
                        ReviewerId = reviewer?.User?.Id,
                        ReviewerName = reviewer?.User?.Fullname,
                        TaskType = task.Type,
                        isMeetingTask = isMeetingTask,
                        meetingId = isMeetingTask ? meetingId : 0,
                        isActive = task.IsActive ?? false,
                        Group = task.Group != null
                            ? new GroupTaskDto { Id = task.Group.Id, Name = task.Group.Name }
                            : null,
                        Milestone = task.Deliverable != null
                            ? new MilestonesDto
                            {
                                Id = task.Deliverable.Id,
                                Name = task.Deliverable.Name,
                                isActive = task.Deliverable.IsActive,
                                Description = task.Deliverable.Description
                            }
                            : null,
                        /*                        Attachments = _context.Attachments?
                                                    .Where(a => a.EntityName.Equals("task") && a.EntityId == task.Id)
                                                    .Select(a => new AttachmentDto
                                                    {
                                                        Id = a.Id,
                                                        FileName = a.FileName,
                                                        FileUrl = a.FilePath
                                                    }).ToList() ?? new List<AttachmentDto>(),*/
                        Comments = task.Comments?
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
                    .Include(t => t.Deliverable)
                    .Include(t => t.Comments)
                    .Include(t => t.TaskUsers)
                        .ThenInclude(tu => tu.User)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    Console.WriteLine($"[GetTaskByIdAsync] Task with ID={taskId} not found.");
                    return null;
                }

                var createdByUser = task.TaskUsers?.FirstOrDefault(tu => tu.Type == "Creator");
                var assignee = task.TaskUsers?.FirstOrDefault(tu => tu.Type == "Assignee");
                var reviewer = task.TaskUsers?.FirstOrDefault(tu => tu.Type == "Reviewer");

                // Xác định isMeetingTask & meetingId
                bool isMeetingTask = task.MeetingScheduleDateId.HasValue;
                int meetingId = isMeetingTask ? task.MeetingScheduleDateId.Value : 0;

                var dto = new TaskDto
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
                    AssigneeId = assignee?.User.Id,
                    AssigneeName = assignee?.User.Fullname,
                    ReviewerId = reviewer?.User?.Id,
                    ReviewerName = reviewer?.User?.Fullname,
                    isMeetingTask = isMeetingTask,
                    meetingId = isMeetingTask ? meetingId : 0,
                    isActive = task.IsActive ?? false,
                    Group = task.Group != null
                        ? new GroupTaskDto { Id = task.Group.Id, Name = task.Group.Name }
                        : null,
                    Milestone = task.Deliverable != null
                            ? new MilestonesDto
                            {
                                Id = task.Deliverable.Id,
                                Name = task.Deliverable.Name,
                                isActive = task.Deliverable.IsActive,
                                Description = task.Deliverable.Description
                            }
                            : null,
                    Attachments = _context.Attachments?
                        .Where(a => a.EntityName.Equals("task") && a.EntityId == taskId)
                        .Select(a => new AttachmentDto
                        {
                            Id = a.Id,
                            FileName = a.FileName,
                            FileUrl = a.FilePath
                        }).ToList() ?? new List<AttachmentDto>(),
                    Comments = task.Comments?
                                .Select(c => new CommentDto
                                {
                                    Id = c.Id,
                                    Author = c.User.RollNumber ?? "",
                                    AuthorName = c.User.Fullname,
                                    Content = c.Feedback ?? "",
                                    Timestamp = c.CreateAt
                                }).ToList() ?? new List<CommentDto>(),
                    History = _context.Logs
                        .Where(h => h.EntityName.Equals("task") && h.EntityId == taskId)
                        .Select(h => new HistoryDto
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
                Console.WriteLine($"[GetTaskByIdAsync] Error while processing TaskId={taskId}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        public async Task<Entities.Models.Task?> UpdateTaskAsync(UpdateTaskDTO dto, int updatedBy)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskUsers)
                .Include(t => t.Deliverable)
                .FirstOrDefaultAsync(t => t.Id == dto.Id);

            if (task == null)
                return null;

            // 📝 Cập nhật thông tin cơ bản của Task
            task.Name = dto.Name;
            task.Description = dto.Description;
            task.Deadline = dto.EndAt;
            task.Status = dto.StatusId;
            task.Priority = dto.PriorityId;
            task.DeliverableId = dto.DeliverableId;
            task.GroupId = dto.GroupId;
            task.MeetingScheduleDateId = dto.MeetingId > 0 ? dto.MeetingId : null;

            // --- Xử lý TaskUsers ---
            // 1️⃣ Người tạo (Creator)
            var creator = task.TaskUsers.FirstOrDefault(tu => tu.Type == "Creator");
            if (creator == null)
            {
                _context.TaskUsers.Add(new TaskUser
                {
                    TaskId = task.Id,
                    UserId = updatedBy,
                    Type = "Creator"
                });
            }
            else
            {
                creator.UserId = updatedBy;
            }

            // 2️⃣ Người được giao (Assignee)
            var assignee = task.TaskUsers.FirstOrDefault(tu => tu.Type == "Assignee");
            if (assignee == null)
            {
                _context.TaskUsers.Add(new TaskUser
                {
                    TaskId = task.Id,
                    UserId = dto.AssignedUserId,
                    Type = "Assignee"
                });
            }
            else
            {
                assignee.UserId = dto.AssignedUserId;
            }

            // 3️⃣ Người review (Reviewer)
            var reviewer = task.TaskUsers.FirstOrDefault(tu => tu.Type == "Reviewer");
            if (dto.ReviewerId != null && dto.ReviewerId > 0)
            {
                if (reviewer == null)
                {
                    _context.TaskUsers.Add(new TaskUser
                    {
                        TaskId = task.Id,
                        UserId = dto.ReviewerId.Value,
                        Type = "Reviewer"
                    });
                }
                else
                {
                    reviewer.UserId = dto.ReviewerId.Value;
                }
            }
            else if (reviewer != null)
            {
                _context.TaskUsers.Remove(reviewer); 
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

        public async Task<object?> GetMeetingScheduleWithTasksAsync(int meetingScheduleId)
        {
            return await _context.MeetingScheduleDates
                .Include(m => m.Tasks)
                .Where(m => m.Id == meetingScheduleId)
                .Select(m => new
                {
                    Id = m.Id,
                    MeetingDate = m.MeetingDate,
                    IsActive = m.IsActive,
                    Description = m.Description,
                    IsMeeting = m.IsMeeting,
                    Tasks = m.Tasks
                        .Where(t => t.Type == "meeting")
                        .Select(t => new
                        {
                            t.Id,
                            t.GroupId,
                            t.Name,
                            t.Description,
                            t.Deadline,
                            t.IsActive
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}
