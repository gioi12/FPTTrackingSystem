using DataTranferObjects.Common.Response;
using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Task;
using Entities.Models;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Repositories.Common.Interfaces;
using Repositories.Staff.Interfaces;
using Repositories.Student.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FPTTrackingSystem.Services.Student.Implements
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly AuthUtils _authUtils;
        private readonly FpttrackingSystemContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IGroupRepository _groupRepository;
        public TaskService(ITaskRepository taskRepository, AuthUtils authUtils, FpttrackingSystemContext context, IHttpContextAccessor httpContextAccessor,IAttachmentRepository attachmentRepository,IWebHostEnvironment env,IGroupRepository groupRepository)
        {
            _taskRepository = taskRepository;
            _authUtils = authUtils;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _attachmentRepository = attachmentRepository;
            _env = env;
            _groupRepository = groupRepository;
        }

        public async Task<Entities.Models.Task> CreateTaskAsync(CreateTaskDTO dto)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var endTimeCookie = _httpContextAccessor.HttpContext?.Request.Cookies["end_Time"];
            if (user.Role == "Student" && (user.Groups == null || !user.Groups.Contains(dto.GroupId)))
                throw new UnauthorizedAccessException("Bạn không có quyền tạo task trong nhóm này.");

            if (dto.GroupId <= 0)
                throw new ArgumentException("GroupId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Task name không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.TaskType))
                throw new ArgumentException("TaskType không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("Status không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Priority))
                throw new ArgumentException("Priority không được để trống.");

            if (dto.EndAt == default)
                throw new ArgumentException("EndAt không được để trống.");
            if (!string.IsNullOrWhiteSpace(endTimeCookie) && DateTime.TryParse(endTimeCookie, out DateTime semesterEndTime))
            {
                if (dto.EndAt <= DateTime.Now)
                    throw new ArgumentException("Task deadline must be greater than the current time.");

                if (dto.EndAt > semesterEndTime)
                    throw new ArgumentException("Task deadline cannot exceed the semester end date.");
            }
            else
            {
                throw new InvalidOperationException("Semester end time cookie not found or invalid.");
            }
            if (dto.AssignedUserId <= 0)
                throw new ArgumentException("AssignedUserId không hợp lệ.");

            if (dto.ReviewerId < 0)
                throw new ArgumentException("ReviewerId không hợp lệ.");

            var validTaskTypes = new[] { "todo", "progress", "done" };
            if (string.IsNullOrWhiteSpace(dto.Status) ||
                !validTaskTypes.Contains(dto.Status.Trim().ToLower()))
                throw new ArgumentException("Invalid TaskType. Allowed values: ToDo, Progress, Done.");
            var taskType = dto.TaskType.Trim().ToLower();
            var status = dto.Status.Trim().ToLower();
            var priority = dto.Priority.Trim().ToLower();

            var validPriorities = new[] { "high", "medium", "low" };
            if (string.IsNullOrWhiteSpace(dto.Priority) ||
                !validPriorities.Contains(dto.Priority.Trim().ToLower()))
                throw new ArgumentException("Invalid Priority. Allowed values: High, Medium, Low.");

            string Capitalize(string s) =>
                string.IsNullOrWhiteSpace(s) ? s : char.ToUpper(s[0]) + s.Substring(1).ToLower();

            var newTask = new Entities.Models.Task
            {
                GroupId = dto.GroupId,
                Name = dto.Name,
                Priority = Capitalize(priority),
                Description = dto.Description,
                Deadline = dto.EndAt,
                Status = Capitalize(status),
                DeliverableId = dto.DeliverableId > 0 ? dto.DeliverableId : null,
                Type = Capitalize(taskType),
                CreatedAt = DateTime.Now,
                IsActive = true,
                MeetingScheduleDateId = dto.MeetingId > 0 ? dto.MeetingId : null
            };

            return await _taskRepository.CreateTaskAsync(
                newTask,
                dto.AssignedUserId ?? 0,
                user.Id ?? 0,
                dto.ReviewerId
            );
        }

        public async Task<ApiResponse<List<TaskDto>>> GetTasksByGroupIdAsync(int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role == "Student" || user.Role == "Supervisor")
            {
                if (user.Groups == null || !user.Groups.Contains(groupId))
                {
                    return new ApiResponse<List<TaskDto>>(403, "Bạn không có quyền xem task của nhóm này.", null);
                }
            }

            var tasks = await _taskRepository.GetTasksByGroupIdAsync(groupId);

            if (tasks == null || !tasks.Any())
            {
                return new ApiResponse<List<TaskDto>>(200, "Không tìm thấy task nào trong nhóm này.", new List<TaskDto>());
            }

            return new ApiResponse<List<TaskDto>>(200, "Lấy danh sách task thành công.", tasks);
        }

        public async Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int taskId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var task = await _taskRepository.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                return new ApiResponse<TaskDto>(200, "Không tìm thấy task với ID này.", new TaskDto());
            }

            if ((user.Role == "Student" || user.Role == "Supervisor") &&
                (user.Groups == null || !user.Groups.Contains(task.Group.Id)))
            {
                return new ApiResponse<TaskDto>(403, "Bạn không có quyền xem task này.", null);
            }

            return new ApiResponse<TaskDto>(200, "Lấy thông tin task thành công.", task);
        }

        public async Task<List<TaskDto>> GetTasksByAssigneeAsync()
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var tasks = await _taskRepository.GetTasksByAssigneeAsync(user.Id ?? 0);

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
                    meetingId = meetingId,
                    isActive = task.IsActive ?? false,
                    Group = task.Group != null
                        ? new GroupTaskDto { Id = task.Group.Id, Name = task.Group.Name }
                        : null
                };
            }).ToList();

            return result;
        }

        public async Task<ApiResponse<TaskResponseUpdateDto>> UpdateTaskAsync(UpdateTaskDTO dto)
        {
            try
            {

                var user = await _authUtils.GetUserInfoFromCookie();
                var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == dto.Id);
                var taskCreator = await _context.TaskUsers
                        .FirstOrDefaultAsync(tu => tu.TaskId == dto.Id && tu.Type == "Creator");
                if (existingTask == null)
                    return new ApiResponse<TaskResponseUpdateDto>(200, "Không tìm thấy task", null);

                if (user.Role == "Student")
                {
                    if (user.Groups == null || !user.Groups.Contains(existingTask.GroupId))
                        return new ApiResponse<TaskResponseUpdateDto>(403, "Bạn không có quyền sửa task của nhóm khác.", null);

                    if (taskCreator == null || taskCreator.UserId != user.Id)
                        return new ApiResponse<TaskResponseUpdateDto>(403, "Bạn chỉ được sửa task do chính mình tạo.", null);
                }

                if (dto.GroupId <= 0)
                    throw new ArgumentException("GroupId không hợp lệ.");

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("Task name không được để trống.");

                if (string.IsNullOrWhiteSpace(dto.StatusId))
                    throw new ArgumentException("TaskType không được để trống.");

                if (string.IsNullOrWhiteSpace(dto.PriorityId))
                    throw new ArgumentException("Priority không được để trống.");

                if (dto.EndAt == default)
                    throw new ArgumentException("EndAt không được để trống.");

                if (dto.AssignedUserId <= 0)
                    throw new ArgumentException("AssignedUserId không hợp lệ.");

                var status = dto.StatusId.Trim().ToLower();
                var priority = dto.PriorityId.Trim().ToLower();

                if (dto.ReviewerId <= 0)
                    throw new ArgumentException("ReviewerId không hợp lệ.");
                var validTaskTypes = new[] { "todo", "inprogress", "done" };
                if (string.IsNullOrWhiteSpace(dto.StatusId) ||
                    !validTaskTypes.Contains(dto.StatusId.Trim().ToLower()))
                    throw new ArgumentException("Invalid TaskType. Allowed values: ToDo, InProgress, Done.");

                var validPriorities = new[] { "high", "medium", "low" };
                if (string.IsNullOrWhiteSpace(priority) ||
                    !validPriorities.Contains(priority.Trim().ToLower()))
                    return ApiResponse<TaskResponseUpdateDto>.Fail("Độ ưu tiên không hợp lệ.", 400);
                var updatedTask = await _taskRepository.UpdateTaskAsync(dto, user.Id ?? 0);
                string Capitalize(string s) =>
                string.IsNullOrWhiteSpace(s) ? s : char.ToUpper(s[0]) + s.Substring(1).ToLower();

                dto.StatusId = Capitalize(status);
                dto.PriorityId = Capitalize(priority);

                var assignedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.AssignedUserId);
                var reviewerUser = dto.ReviewerId != null
                    ? await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.ReviewerId)
                    : null;

                var responseDto = new TaskResponseUpdateDto
                {
                    Id = updatedTask.Id,
                    Name = updatedTask.Name,
                    Description = updatedTask.Description,
                    Deadline = updatedTask.Deadline,
                    StatusId = updatedTask.Status,
                    PriorityId = updatedTask.Priority,
                    MilestoneId = updatedTask.DeliverableId,
                    GroupId = updatedTask.GroupId,
                    AssignedUserId = dto.AssignedUserId,
                    AssignedUserName = assignedUser?.Fullname,
                    ReviewerId = dto.ReviewerId,
                    ReviewerName = reviewerUser?.Fullname,
                    MeetingId = dto.MeetingId
                };

                return new ApiResponse<TaskResponseUpdateDto>(200, "Cập nhật thành công", responseDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateTaskAsync] Error: {ex.Message}");
                return new ApiResponse<TaskResponseUpdateDto>(500, "Lỗi khi cập nhật task: " + ex.Message);
            }
        }

        public async Task<object?> GetMeetingScheduleWithTasksAsync(int meetingScheduleId)
        {
            return await _taskRepository.GetMeetingScheduleWithTasksAsync(meetingScheduleId);
        }

        public async Task<string> UploadFileTask(IFormFile file, int groupId, int taskId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var group = await _groupRepository.GetByIdAsync(groupId);
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                throw new ValidationException("Not found task");
            if (group == null)
                throw new ValidationException("Not found group");

            string path = await FileUploadUtils.UploadFileAsync(file, (int)FileEnum.Task, _env);
            // 1. Milestone(Deliverable) , 2 Task , 3 Groups (Documents)
            string entityName = FileUploadUtils.GetEntityName((int)FileEnum.Task);
            Entities.Models.Attachment attachment = new Entities.Models.Attachment()
            {
                CreateAt = DateTime.Now,
                FileName = file.FileName,
                FilePath = path,
                EntityName = entityName,
                EntityId = taskId,
                GroupId = groupId,
                UserId = (int)user.Id,
                IsDownload = false
            };
            await _attachmentRepository.AddAttachment(attachment);
            return path;
        }

        public async System.Threading.Tasks.Task DeleteFileTask(int attachmentId)
        {
            var attachment = await _attachmentRepository.GetAttachmentById(attachmentId);
            if (attachment == null) throw new ValidationException("Not found attachment");
            await _attachmentRepository.DeleteAttachment(attachment);
        }

        public async Task<List<AttachmentRes>> GetFilesTask(int groupId,int taskId)
        {
            var entityName = FileUploadUtils.GetEntityName((int)FileEnum.Task);
            var allAttachments = await _attachmentRepository.GetAttachments(
                entityName, groupId, taskId
            );

            return allAttachments.Adapt<List<AttachmentRes>>();
        }
    }
}
