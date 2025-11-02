using Azure;
using DataTranferObjects.Common.Response;
using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Helper;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repositories.Authentication;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IAccountRepository _accountRepository;

        public GroupService(IGroupRepository groupRepository, AuthUtils authUtils, IWebHostEnvironment env,IAttachmentRepository attachmentRepository, IHttpContextAccessor httpContextAccessor,IAccountRepository accountRepository,ISemesterRepository semesterRepository)
        {
            _groupRepository = groupRepository;
            _authUtils = authUtils;
            _env = env;
            _attachmentRepository = attachmentRepository;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
            _semesterRepository = semesterRepository;
        }

        public async Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _groupRepository.GetGroupsQuery();
            if (user.Role == "Student" || user.Role == "Supervisor" || user.Role == "SupervisorHead")
            {
                if (user.Groups == null || !user.Groups.Any())
                    return new PagedResponse<GroupDto>
                    {
                        Status = 200,
                        Message = "Không có nhóm nào thuộc quyền của bạn.",
                        Data = new PagedData<GroupDto> { Items = new List<GroupDto>(), Total = 0 }
                    };

                query = query.Where(g => user.Groups.Contains(g.Id));
            }
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
            var user = await _authUtils.GetUserInfoFromCookie();


            var semesterIdCookie = _httpContextAccessor.HttpContext?.Request.Cookies["semesterId"];
            if (string.IsNullOrEmpty(semesterIdCookie))
            {
                return new ApiResponse<GroupDetailDto>(400, "Current semester information not found in cookie.", null);
            }

            if (!int.TryParse(semesterIdCookie, out int currentSemesterId))
            {
                return new ApiResponse<GroupDetailDto>(400, "Invalid current semester value in cookie.", null);
            }

            var group = await _groupRepository.GetByIdAsync(id);

            if (group == null)
            {
                return new ApiResponse<GroupDetailDto>(200, "Group not found.", null);
            }

            if (group.SemesterId != currentSemesterId && user.Role != RoleEnum.Staff.ToString())
            {
                return new ApiResponse<GroupDetailDto>(200, "This group does not belong to the current semester.", null);
            }
            if (user.Role == "Student" || user.Role == "Supervisor" || user.Role == "SupervisorHead")
            {
                if (user.Groups == null || !user.Groups.Contains(id))
                {
                    return new ApiResponse<GroupDetailDto>(403, "Bạn không có quyền truy cập nhóm này.", new GroupDetailDto());
                }
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
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.Role == "Student" || user.Role == "Supervisor" || user.Role == "SupervisorHead")
            {
                if (user.Groups == null || !user.Groups.Contains(groupId))
                    return new ApiResponse<GroupTrackingResponseDto>(403, "Bạn không có quyền xem nhóm này.", null);
            }
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

        public async Task<ApiResponse<List<GroupMentorDto>>> GetGroupsByUserIdAsync(int userId)
        {
            var currentUser = await _authUtils.GetUserInfoFromCookie();
            var groups = await _groupRepository.GetGroupsByUserIdAsync(userId);
            if (currentUser.Role == "Student")
            {
                // Chỉ xem nhóm của chính mình
                if (currentUser.Id != userId)
                {
                    return new ApiResponse<List<GroupMentorDto>>
                    {
                        Status = 403,
                        Message = "Bạn không có quyền xem nhóm của người dùng khác.",
                        Data = null
                    };
                }
            }
            else if (currentUser.RoleInGroup == StringEnum.Supervior || currentUser.RoleInGroup == "SupervisorHead")
            {
                groups = groups
                    .Where(g => g.GroupUsers.Any(gu =>
                        gu.UserId == currentUser.Id &&
                        (gu.User.Account.RoleId == (int)RoleEnum.Supervior ||
                         gu.User.Account.RoleId == (int)RoleEnum.SuperviorHead)))
                    .ToList();

                if (!groups.Any())
                {
                    return new ApiResponse<List<GroupMentorDto>>
                    {
                        Status = 403,
                        Message = "Bạn không hướng dẫn nhóm nào trong danh sách này.",
                        Data = null
                    };
                }
            }
            var result = groups.Select(g => new GroupMentorDto
            {
                Id = g.Id,
                GroupCode = g.Code,
                Name = g.Name,
                status = g.Status != null ? g.Status.Name : "active",
                students = g.GroupUsers
                    .Where(gu => gu.Role == StringEnum.Student || gu.Role == StringEnum.Secretary || gu.Role == StringEnum.Leader && gu.IsActive)
                    .Select(s => new StudentGroupDTO
                    {
                        Id = s.User.Id,
                        Name = s.User.Fullname,
                        Email = s.User.Mail,
                        RollNumber = s.User.RollNumber
                    })
                    .ToList()
            }).ToList();

            return new ApiResponse<List<GroupMentorDto>>
            {
                Status = 200,
                Message = "Lấy danh sách nhóm thành công.",
                Data = result
            };
        }


        public async Task<ApiResponse<string>> UpdateRoleInGroupAsync(int groupId, int userId, string newRole)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user.RoleInGroup != "Secretary" && user.Role != "SupervisorHead" && user.Role != "Supervisor")
                return new ApiResponse<string>(403, "Bạn không có quyền thay đổi vai trò trong nhóm.", null);

            try
            {
                var success = await _groupRepository.UpdateRoleInGroupAsync(groupId, userId, newRole);

                if (!success)
                    return new ApiResponse<string>
                    {
                        Status = 200,
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

        public Task<object> GetMockData()
        {
            // Map RollNumber -> UserId giả
            var userIdMap = new Dictionary<string, int>
    {
        {"SE150001", 1},
        {"SE150002", 2},
        {"SE150003", 3},
        {"SE150004", 4},
        {"ME01", 5},
        {"ME02", 6}
    };

            // Lấy tất cả users từ Accounts với Id giả
            var allUsers = MockData.Accounts
                .Where(a => a.Users != null && a.Users.Any())
                .SelectMany(a => a.Users)
                .Select(u => new
                {
                    Id = userIdMap.ContainsKey(u.RollNumber) ? userIdMap[u.RollNumber] : 0,
                    u.Fullname,
                    u.RollNumber,
                    u.Mail,
                    u.Phone
                })
                .ToList();

            // Tạo groups với IDs tương ứng
            var groups = MockData.GetGroups(1, 1, 2, 3, 4, 5, 6);

            var result = groups.Select(group => new
            {
                GroupCode = group.Code,
                GroupName = group.Name,
                MajorId = group.MajorId,
                Profession = group.Profession,
                VietnameseTitle = group.VietnameseTitle,
                Description = group.Description,
                Status = group.StatusId,
                Members = group.GroupUsers.Select(gu =>
                {
                    var user = allUsers.FirstOrDefault(u => u.Id == gu.UserId);
                    return new
                    {
                        UserId = user?.Id,
                        Fullname = user?.Fullname,
                        RollNumber = user?.RollNumber,
                        Email = user?.Mail,
                        Phone = user?.Phone,
                        RoleInGroup = gu.Role,
                        IsActive = gu.IsActive,
                        Status = gu.Status
                    };
                }).ToList()
            }).ToList();

            return System.Threading.Tasks.Task.FromResult<object>(result);
        }



        public async Task<object> CreateMockData()
        {
            var semester = await _semesterRepository.findActive();
            if (semester == null)
            {
                throw new ValidationException("Chưa có kỳ học active");
            }

            // 1. Lấy mock data
            var accounts = MockData.Accounts;

            // 2. Tạo accounts (sau khi SaveChanges, accounts đã có Id rồi!)
            await _accountRepository.CreateUsers(accounts);

            // 3. Dùng lại accounts - Id đã được tự động fill
            var user1 = accounts[0].Users.FirstOrDefault();
            var user2 = accounts[1].Users.FirstOrDefault();
            var user3 = accounts[2].Users.FirstOrDefault();
            var user4 = accounts[3].Users.FirstOrDefault();
            var mentor1 = accounts[4].Users.FirstOrDefault();
            var mentor2 = accounts[5].Users.FirstOrDefault();

            // 4. Tạo groups với UserId đã có
            var groups = new List<Group>
    {
        new Group
        {
            Code = "G01",
            Name = "Capstone Team Alpha",
            SemesterId = semester.Id,
            CreateAt = DateTime.Now.AddMonths(-2),
            Profession = "AI Development",
            MajorId = 1,
            Description = "Team làm chatbot AI",
            VietnameseTitle = "Nhóm Alpha",
            StatusId = "ACTIVE",
            GroupUsers = new List<GroupUser>
            {
                new GroupUser
                {
                    UserId = user1.Id,
                    Role = "Leader",
                    IsActive = true,
                    CreateAt = DateTime.Now.AddMonths(-2),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new GroupUser
                {
                    UserId = user2.Id,
                    Role = "Member",
                    IsActive = true,
                    CreateAt = DateTime.Now.AddMonths(-2),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new GroupUser
                {
                    UserId = mentor1.Id,
                    Role = "Supervisor",
                    IsActive = true,
                    CreateAt = DateTime.Now.AddMonths(-1),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                }
            }
        },
        new Group
        {
            Code = "G02",
            Name = "Chiến dịch quảng cáo xanh",
            SemesterId = semester.Id,
            CreateAt = DateTime.Now.AddMonths(-1),
            Profession = "Marketing",
            MajorId = 2,
            Description = "Team xây dựng plan marketing",
            VietnameseTitle = "Nhóm Marketing",
            StatusId = "ACTIVE",
            GroupUsers = new List<GroupUser>
            {
                new GroupUser
                {
                    UserId = user3.Id,
                    Role = "Leader",
                    IsActive = true,
                    CreateAt = DateTime.Now.AddMonths(-1),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new GroupUser
                {
                    UserId = user4.Id,
                    Role = "Member",
                    IsActive = true,
                    CreateAt = DateTime.Now.AddMonths(-1),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                },
                new GroupUser
                {
                    UserId = mentor2.Id,
                    Role = "Supervisor",
                    IsActive = true,
                    CreateAt = DateTime.Now.AddMonths(-1),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                }
            }
        }
    };

            await _groupRepository.CreateGroups(groups);

            return "Create mock data successfully";
        }
    }

}
