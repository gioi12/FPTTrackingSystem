using Azure;
using DataTranferObjects.Common.Response;
using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Repositories.Common.Interfaces;
using Repositories.Staff.Implements;
using Repositories.Staff.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FPTTrackingSystem.Services.Staff.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly AuthUtils _authUtils;
        private readonly IWebHostEnvironment _env;
        private readonly IAttachmentRepository _attachmentRepository;

        public GroupService(IGroupRepository groupRepository, AuthUtils authUtils, IWebHostEnvironment env,IAttachmentRepository attachmentRepository)
        {
            _groupRepository = groupRepository;
            _authUtils = authUtils;
            _env = env;
            _attachmentRepository = attachmentRepository;
        }

        public async Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _groupRepository.GetGroupsQuery();
            var total = await _groupRepository.CountAsync(query);

            var groups = await query
                .OrderBy(g => g.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                 .Select(g => new GroupDto
                 {
                     Id = g.Id.ToString(),
                     CourseCode = g.Name,
                     GroupCode = g.Code,
                     Term = g.Semester != null ? g.Semester.Name : "",
                     Major = g.Major != null ? g.Major.Name : "",
                     StudentCount = g.GroupUsers.Count(gu => gu.User.Account.RoleId == (int)RoleEnum.Student),
                     Supervisor = g.GroupUsers
                        .Where(gu => gu.User.Account.RoleId == (int)RoleEnum.Supervior || gu.User.Account.RoleId == (int)RoleEnum.SuperviorHead)
                        .Select(gu => gu.User.Fullname)
                        .ToList(),
                     SubmittedDocs = false,
                 })
                  .ToListAsync();

            return new PagedResponse<GroupDto>
            {
                Status = 200,
                Message = "Lấy thành công",
                Data = new PagedData<GroupDto>
                {
                    Items = groups,
                    Total = total
                }
            };
        }

        public async Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
            {
                return new ApiResponse<GroupDetailDto>
                {
                    Status = 200,
                    Message = "Không tìm thấy nhóm",
                    Data = null
                };
            }
            var dto = new GroupDetailDto
            {
                Id = group.Id.ToString(),
                ProjectName = group.Name,
                GroupCode = group.Code,
                SemesterId = group.SemesterId,
                Supervisors = group.GroupUsers
                    .Where(gu => gu.User != null
                              && gu.User.Account != null
                              && (gu.User.Account.RoleId == (int)RoleEnum.Supervior
                               || gu.User.Account.RoleId == (int)RoleEnum.SuperviorHead))
                    .Select(gu => gu.User.Fullname)
                    .ToList(),
                SupervisorsInfor = group.GroupUsers
                    .Where(gu => gu.User != null
                              && gu.User.Account != null
                              && (gu.User.Account.RoleId == (int)RoleEnum.Supervior
                               || gu.User.Account.RoleId == (int)RoleEnum.SuperviorHead))
                    .Select(gu => new SuperviorDto
                    {
                        Id = gu.User.Id,
                        Name = gu.User.Fullname,
                        Email = gu.User.Mail

                    })
                    .ToList(),
                Status = group.Status?.Name,
                Risk = "Low",
                Students = group.GroupUsers
                    .Where(gu => gu.User != null
                              && gu.User.Account != null
                              && gu.User.Account.RoleId == (int)RoleEnum.Student)
                    .Select(gu => new StudentDto
                    {
                        Id = gu.User.Id,
                        RollNumber = gu.User.RollNumber,
                        Name = gu.User.Fullname,
                        Email = gu.User.Mail,
                        Role = gu.Role
                    }).ToList(),

                ActivityLog = null,
            };


            return new ApiResponse<GroupDetailDto>
            {
                Status = 200,
                Message = "Lấy thành công",
                Data = dto
            };
        }

        public async Task<ApiResponse<List<DashBoardGroupDto>>> GetMajorGroupTotalsAsync()
        {
            var data = await _groupRepository.GetMajorGroupTotalsAsync();
            return new ApiResponse<List<DashBoardGroupDto>>(200, "Lấy thành công", data);
        }

        public async Task<ApiResponse<GroupTrackingResponseDto>> GetGroupTrackingAsync(int groupId, DateTime startDate, DateTime endDate)
        {
            var group = await _groupRepository.GetGroupWithMembersAsync(groupId);
            if (group == null)
            {
                return new ApiResponse<GroupTrackingResponseDto>
                {
                    Status = 200,
                    Message = "Không tìm thấy nhóm",
                    Data = null
                };
            }

            var milestones = await _groupRepository.GetMilestonesByMajorAsync(group.MajorId ?? 0);

            // ============ Tạo danh sách Week ============
            List<WeekDto> weeks = new();
            DateTime semesterStart = new DateTime(startDate.Year, 9, 1);
            DateTime semesterEnd = new DateTime(startDate.Year, 12, 31);

            int weekIndex = 1;
            for (DateTime weekStart = semesterStart; weekStart <= semesterEnd; weekStart = weekStart.AddDays(7))
            {
                DateTime weekEnd = weekStart.AddDays(6);
                if (weekEnd > semesterEnd) weekEnd = semesterEnd;

                string label = $"Week {weekIndex}: {weekStart:dd/MM/yyyy} - {weekEnd:dd/MM/yyyy}";
                weeks.Add(new WeekDto { Value = label, Label = label });
                weekIndex++;
            }

            string currentWeek = weeks.FirstOrDefault(w =>
            {
                var parts = w.Value.Split(':')[1].Trim().Split(" - ");
                DateTime ws = DateTime.ParseExact(parts[0], "dd/MM/yyyy", null);
                DateTime we = DateTime.ParseExact(parts[1], "dd/MM/yyyy", null);
                return startDate >= ws && endDate <= we;
            })?.Value ?? $"Week ?: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";

            // ============ TimeSlots cố định ============
            var timeSlots = new List<string>
            {
                "00:00 - 04:00",
                "04:00 - 08:00",
                "08:00 - 12:00",
                "12:00 - 16:00",
                "16:00 - 20:00",
                "20:00 - 24:00"
            };

            // ============ Days of week ============
            var days = Enumerable.Range(0, 7)
                .Select(i => new DayDto
                {
                    Name = startDate.AddDays(i).ToString("dddd"),
                    Date = startDate.AddDays(i).ToString("dd/MM")
                }).ToList();

            // ============ Group Members ============
            var groupMembers = group.GroupUsers
                .Where(gu => gu.User != null && gu.User.Account != null && gu.User.Account.RoleId == (int)RoleEnum.Student)
                .Select(gu => new GroupMemberDto
                {
                    Id = gu.User.RollNumber,
                    Name = gu.User.Fullname,
                    IsLeader = gu.User.Account.RoleId == (int)RoleEnum.StudentLead
                }).ToList();

            // ============ Milestones ============
            var milestoneDtos = milestones.Select(m => new MilestoneDto
            {
                Name = m.Name ?? "",
                Deadline = DateTime.TryParse(m.Deadline, out var d) ? d : null,
                Status = "not-submitted"
            }).ToList();

            var dto = new GroupTrackingResponseDto
            {
                CurrentWeek = currentWeek,
                Weeks = weeks,
                TimeSlots = timeSlots,
                Days = days,
                GroupMembers = groupMembers,
                Milestones = milestoneDtos
            };

            return new ApiResponse<GroupTrackingResponseDto>
            {
                Status = 200,
                Message = "Lấy dữ liệu tracking thành công",
                Data = dto
            };
        }

        public async Task<List<GroupMentorDto>> GetGroupsByUserIdAsync(int userId)
        {
            var groups = await _groupRepository.GetGroupsByUserIdAsync(userId);

            var result = groups.Select(g => new GroupMentorDto
            {
                Id = g.Id,
                GroupCode = g.Code,
                Name = g.Name,
                status = g.Status != null ? g.Status.Name : "active",
                students = g.GroupUsers
                    .Where(gu => gu.Role == "Student" || gu.Role == "Secretary" || gu.Role == "Leader" && gu.IsActive)
                    .Select(s => new StudentGroupDTO
                    {
                        Id = s.User.Id,
                        Name = s.User.Fullname,
                        Email = s.User.Mail,
                        RollNumber = s.User.RollNumber
                    })
                    .ToList()
            }).ToList();

            return result;
        }


        public async Task<ApiResponse<string>> UpdateRoleInGroupAsync(int groupId, int userId, string newRole)
        {
            try
            {
                var success = await _groupRepository.UpdateRoleInGroupAsync(groupId, userId, newRole);

                if (!success)
                    return new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Không tìm thấy nhóm hoặc sinh viên.",
                        Data = null
                    };

                return new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Cập nhật role trong group thành công.",
                    Data = null
                };
            }
            catch (InvalidOperationException ex)
            {
                return new ApiResponse<string>
                {
                    Status = 400,
                    Message = ex.Message,
                    Data = null
                };
            }
            catch (Exception)
            {
                return ApiResponse<string>.Fail("Đã xảy ra lỗi khi cập nhật role trong group.");
            }
        }

        public async Task<string> UploadFileGroup(IFormFile file, int groupId)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new ValidationException("Not found group");

            string path = await FileUploadUtils.UploadFileAsync(file, (int)FileEnum.Group, _env);
            // 1. Milestone(Deliverable) , 2 Task , 3 Groups (Documents)
            string entityName = FileUploadUtils.GetEntityName((int)FileEnum.Group);
            Entities.Models.Attachment attachment = new Entities.Models.Attachment()
            {
                CreateAt = DateTime.Now,
                FileName = file.FileName,
                FilePath = path,
                EntityName = entityName,
                EntityId = groupId,
                GroupId = groupId,
                UserId = (int)user.Id,
                IsDownload = false
            };
            await _attachmentRepository.AddAttachment(attachment);
            return path;
        }

        public async System.Threading.Tasks.Task DeleteFileGroup(int attachmentId)
        {
            var attachment = await _attachmentRepository.GetAttachmentById(attachmentId);
            if (attachment == null) throw new ValidationException("Not found attachment");
            await _attachmentRepository.DeleteAttachment(attachment);
        }

        public async Task<List<AttachmentRes>> GetFilesGroup(int groupId)
        {
            var entityName = FileUploadUtils.GetEntityName((int)FileEnum.Group);
            var allAttachments = await _attachmentRepository.GetAttachments(
                entityName, groupId, groupId
            );

            return allAttachments.Adapt<List<AttachmentRes>>();
        }
    }

}
