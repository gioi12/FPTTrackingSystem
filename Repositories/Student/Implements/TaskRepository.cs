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
        public async Task<List<Entities.Models.Task>> GetAllActiveMeetingTasksAsync(int groupId)
        {
            return await _context.Tasks
                .Where(t => t.GroupId == groupId &&
                t.Type == "Meeting"
                            && (t.Status == "Todo" || t.Status == "InProgress")
                            && t.IsActive == true)
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }

        public async Task<Entities.Models.Task?> GetByIdAsync(int taskId)
        {
            return await _context.Tasks
                .Include(t => t.TaskUsers)
                .Include(t => t.Comments)
                .Include(t => t.MeetingMinute)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async System.Threading.Tasks.Task DeleteWithRelationAsync(Entities.Models.Task task)
        {
            // Xóa TaskUsers
            if (task.TaskUsers.Any())
                _context.TaskUsers.RemoveRange(task.TaskUsers);

            // Xóa Comments
            if (task.Comments.Any())
                _context.Comments.RemoveRange(task.Comments);

            // Xóa liên kết MeetingMinute
            if (task.MeetingMinute != null)
            {
                task.MeetingMinute.Tasks.Remove(task);
            }

            // Xóa Task
            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Entities.Models.Task>> GetTasksByReviewerAsync(int userId, int groupId)
        {
            return await _context.Tasks
                .Include(t => t.TaskUsers)
                .Where(t =>
                    t.GroupId == groupId &&
                    t.TaskUsers.Any(tu =>
                        tu.UserId == userId &&
                        tu.Type == "Reviewer"
                    )
                )
                .ToListAsync();
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

        public async Task<List<Entities.Models.Task>> GetTasksByAssigneeStatisticalAsync(int userId)
        {
            return await _context.Tasks
                        .Include(t => t.TaskUsers)
                        .Where(t => t.TaskUsers.Any(tu => tu.UserId == userId && tu.Type == "Assignee"))
                        .ToListAsync();
        }

        public async Task<List<Entities.Models.Task>> GetTaskTypeIssueAsync(int groupId)
        {
            return await _context.Tasks
                .Where(t => t.GroupId == groupId && t.Type == "Meeting")
                .ToListAsync();
        }

        public async Task<List<TaskDto>> GetTasksByGroupIdAsync(int groupId)
        {
            try
            {
                // 1️ Lấy task cơ bản
                var tasks = await _context.Tasks
                    .Where(t => t.GroupId == groupId)
                    .OrderBy(t => t.Deadline)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Description,
                        t.Deadline,
                        t.CreatedAt,
                        t.Priority,
                        t.Status,
                        t.Type,
                        t.IsActive,
                        t.MeetingMinuteId,
                        Group = new { t.Group.Id, t.Group.Name },
                        Milestone = t.Deliverable == null ? null : new
                        {
                            t.Deliverable.Id,
                            t.Deliverable.Name,
                            t.Deliverable.Description,
                            t.Deliverable.IsActive
                        }
                    })
                    .ToListAsync();

                if (!tasks.Any()) return new List<TaskDto>();

                var taskIds = tasks.Select(t => t.Id).ToList();

                // 2️ Lấy TaskUsers
                var taskUsers = await _context.TaskUsers
                    .Include(tu => tu.User)
                    .Where(tu => taskIds.Contains(tu.TaskId))
                    .ToListAsync();

                // 3️ Lấy Comments
                var comments = await _context.Comments
                    .Include(c => c.User)
                    .Where(c => taskIds.Contains(c.TaskId ?? 0))
                    .ToListAsync();

                // 4️ Lấy Logs
                var logs = await _context.Logs
                    .Include(l => l.User)
                    .Where(l => l.EntityName == "task" && taskIds.Contains(l.EntityId))
                    .ToListAsync();

                // 5️ Map result
                var result = tasks.Select(t =>
                {
                    var taskUserList = taskUsers.Where(u => u.TaskId == t.Id);
                    var createdBy = taskUserList.FirstOrDefault(u => u.Type == "Creator");
                    var assignee = taskUserList.FirstOrDefault(u => u.Type == "Assignee");
                    var reviewer = taskUserList.FirstOrDefault(u => u.Type == "Reviewer");

                    return new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Name,
                        Description = t.Description,
                        Deadline = t.Deadline,
                        CreatedAt = t.CreatedAt,
                        Priority = t.Priority,
                        Status = t.Status,
                        TaskType = t.Type,
                        isActive = t.IsActive ?? false,
                        CreatedBy = createdBy?.User?.Id,
                        CreatedByName = createdBy?.User?.Fullname,
                        AssigneeId = assignee?.User?.Id,
                        AssigneeName = assignee?.User?.Fullname,

                        ReviewerId = reviewer?.User?.Id,
                        ReviewerName = reviewer?.User?.Fullname,

                        isMeetingTask = t.MeetingMinuteId.HasValue,
                        meetingId = t.MeetingMinuteId ?? 0,

                        Group = t.Group == null ? null : new GroupTaskDto
                        {
                            Id = t.Group.Id,
                            Name = t.Group.Name
                        },

                        Milestone = t.Milestone == null ? null : new MilestonesDto
                        {
                            Id = t.Milestone.Id,
                            Name = t.Milestone.Name,
                            Description = t.Milestone.Description,
                            isActive = t.Milestone.IsActive
                        },

                        Comments = comments
                            .Where(c => c.TaskId == t.Id)
                            .Select(c => new CommentDto
                            {
                                Id = c.Id,
                                Author = c.User.RollNumber,
                                AuthorName = c.User.Fullname,
                                Content = c.Feedback,
                                Timestamp = c.CreateAt
                            }).ToList(),

                        History = logs
                            .Where(h => h.EntityId == t.Id)
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
                Console.WriteLine(ex);
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
                bool isMeetingTask = task.MeetingMinuteId.HasValue;
                int meetingId = isMeetingTask ? task.MeetingMinuteId.Value : 0;

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
            task.MeetingMinuteId = dto.MeetingId > 0 ? dto.MeetingId : null;

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
            return task;
        }

        public async Task<object?> GetMeetingScheduleWithTasksAsync(int meetingScheduleId)
        {
            return await _context.MeetingMinutes
                .Include(m => m.MeetingScheduleDate)
                .Include(m => m.Tasks)
                .Where(m => m.Id == meetingScheduleId)
                .Select(m => new
                {
                    Id = m.Id,
                    MeetingDate = m.MeetingScheduleDate.MeetingDate,
                    IsActive = m.MeetingScheduleDate.IsActive,
                    Description = m.MeetingScheduleDate.Description,
                    IsMeeting = m.MeetingScheduleDate.IsMeeting,
                    Tasks = m.Tasks
                        .Where(t => t.Type == "meeting")
                        .Select(t => new
                        {
                            t.Id,
                            t.GroupId,
                            t.Name,
                            t.Description,
                            t.Deadline,
                            t.IsActive,
                            t.Status
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}
