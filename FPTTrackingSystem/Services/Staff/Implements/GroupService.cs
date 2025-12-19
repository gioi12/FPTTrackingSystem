using Azure;
using DataTranferObjects.Common.Response;
using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Helper;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Services.Token;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Authentication;
using Repositories.Common.Interfaces;
using Repositories.Staff.Implements;
using Repositories.Staff.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Group = Entities.Models.Group;


namespace FPTTrackingSystem.Services.Staff.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IMajorRepository _majorRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly AuthUtils _authUtils;
        private readonly IWebHostEnvironment _env;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISemesterRepository _semesterRepository;
        private readonly ISemesterService _semesterService;
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;

        public GroupService(IGroupRepository groupRepository, IJwtService jwtService,IMajorRepository majorRepository, ISemesterService semesterService,  AuthUtils authUtils, IWebHostEnvironment env,IAttachmentRepository attachmentRepository, IHttpContextAccessor httpContextAccessor,IAccountRepository accountRepository,ISemesterRepository semesterRepository)
        {
            _groupRepository = groupRepository;
            _authUtils = authUtils;
            _env = env;
            _attachmentRepository = attachmentRepository;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
            _semesterRepository = semesterRepository;
            _majorRepository = majorRepository;
            _jwtService = jwtService;
            _semesterService = semesterService;
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
                {
                    return new PagedResponse<GroupDto>
                    {
                        Status = 200,
                        Message = "Không có nhóm nào thuộc quyền của bạn.",
                        Data = new PagedData<GroupDto>
                        {
                            Items = new List<GroupDto>(),
                            Total = 0
                        }
                    };
                }

                var userGroupIds = user.Groups.ToList();

                query = query.Where(g => userGroupIds.Contains(g.Id ?? 0));
            }

            var total = await query.CountAsync();

            var groups = await query
                .OrderBy(g => g.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var g in groups)
            {
                g.Supervisor = g.Supervisor?.ToList() ?? new List<string>();
            }

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

        public async Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int groupId)
        {
            try
            {
                var user = await _authUtils.GetUserInfoFromCookie();
                if (user == null)
                {
                    return new ApiResponse<GroupDetailDto>(401, "User not authenticated.", null);
                }

                bool roleStaff = user.Role?.EndsWith(RoleEnum.Staff.ToString()) ?? false;

                List<GroupMentorDto> accessibleGroups;

                var activeGroups = (await GetGroupsByUserIdAsync(user.Id ?? 0)).Data ?? new List<GroupMentorDto>();
                var expiredGroups = await _groupRepository.GetExpiredGroupsByUserIdAsync(user.Id ?? 0);

                accessibleGroups = activeGroups
                    .Concat(expiredGroups)
                    .GroupBy(g => g.Id)
                    .Select(g => g.First())
                    .ToList();

                bool inAccessibleGroups = accessibleGroups.Any(g => g.Id == groupId);
                bool inUserGroups = user.Groups?.Contains(groupId) ?? false;

                if (!roleStaff)
                {
                    if (!inAccessibleGroups && !inUserGroups)
                    {
                        return new ApiResponse<GroupDetailDto>(403, "Bạn không có quyền xem nhóm này.", null);
                    }
                }

                var group = await _groupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return new ApiResponse<GroupDetailDto>(404, "Group not found.", null);
                }

                var semesterName = group.Semester?.Name ?? "Unknown";

                var dto = new GroupDetailDto
                {
                    Id = group.Id.ToString(),
                    ProjectName = group.Name,
                    SemesterName = semesterName,
                    GroupCode = group.Code,
                    SemesterId = group.SemesterId,
                    IsExpired = group.ExpireDate != null && group.ExpireDate < DateTime.UtcNow,
                    ExpireDate = group.ExpireDate,

                    Supervisors = group.GroupUsers
                        .Where(gu => gu.User != null &&
                            (gu.Role == "Supervisor" || gu.Role == "SupervisorHead"))
                        .Select(gu => gu.User.Fullname)
                        .ToList(),

                    SupervisorsInfor = group.GroupUsers
                        .Where(gu => gu.User != null &&
                            (gu.Role == "Supervisor" || gu.Role == "SupervisorHead"))
                        .Select(gu => new SuperviorDto
                        {
                            Id = gu.User.Id,
                            Name = gu.User.Fullname,
                            Email = gu.User.Mail
                        }).ToList(),

                    Status = group.Status?.Name,
                    Risk = "Low",

                    Students = group.GroupUsers
                        .Where(gu => gu.User != null &&
                            (gu.Role == "Student" || gu.Role == "Leader" || gu.Role == "Secretary"))
                        .Select(gu => new StudentDto
                        {
                            Id = gu.User.Id,
                            RollNumber = gu.User.RollNumber,
                            Name = gu.User.Fullname,
                            Email = gu.User.Mail,
                            Role = gu.Role
                        }).ToList(),

                    ActivityLog = null
                };

                return new ApiResponse<GroupDetailDto>(200, "Lấy thành công", dto);
            }
            catch (Exception ex)
            {
                // LOG CHI TIẾT
                Console.WriteLine($"[GetGroupByIdAsync] ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                return new ApiResponse<GroupDetailDto>(500, "Internal Server Error: " + ex.Message, null);
            }
        }


        /*public async Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int groupId)
        {
            // Lấy user
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
            {
                return new ApiResponse<GroupDetailDto>(401, "User not authenticated.", null);
            }
            List<GroupMentorDto> accessibleGroups;
            // Lấy tất cả nhóm user được truy cập -> DÙNG HÀM NÀY
            var activeGroups = (await GetGroupsByUserIdAsync(user.Id ?? 0)).Data ?? new List<GroupMentorDto>();
            var expiredGroups = await _groupRepository.GetExpiredGroupsByUserIdAsync(user.Id ?? 0);

            accessibleGroups = activeGroups
           .Concat(expiredGroups)
           .GroupBy(g => g.Id)
           .Select(g => g.First())
           .ToList();
            bool inAccessibleGroups = accessibleGroups.Any(g => g.Id == groupId);
            bool inUserGroups = user.Groups?.Contains(groupId) ?? false;
            bool roleStaff = user.Role.EndsWith(RoleEnum.Staff.ToString());

            if (!roleStaff)
            {
                if (!inAccessibleGroups && !inUserGroups)
                {
                    return new ApiResponse<GroupDetailDto>(403, "Bạn không có quyền xem nhóm này.", null);
                }
            }

            // Lấy group từ DB
            var group = await _groupRepository.GetByIdAsync(groupId);

            if (group == null)
            {
                return new ApiResponse<GroupDetailDto>(404, "Group not found.", null);
            }

            var dto = new GroupDetailDto
            {
                Id = group.Id.ToString(),
                ProjectName = group.Name,
                SemesterName = group.Semester.Name,
                GroupCode = group.Code,
                SemesterId = group.SemesterId,
                IsExpired = group.ExpireDate != null && group.ExpireDate < DateTime.UtcNow,
                ExpireDate = group.ExpireDate,
                Supervisors = group.GroupUsers
                    .Where(gu => gu.User != null &&
                        (gu.Role == "Supervisor" || gu.Role == "SupervisorHead"))
                    .Select(gu => gu.User.Fullname)
                    .ToList(),

                SupervisorsInfor = group.GroupUsers
                    .Where(gu => gu.User != null &&
                        (gu.Role == "Supervisor" || gu.Role == "SupervisorHead"))
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
                    .Where(gu => gu.User != null &&
                        (gu.Role == "Student" || gu.Role == "Leader" || gu.Role == "Secretary"))
                    .Select(gu => new StudentDto
                    {
                        Id = gu.User.Id,
                        RollNumber = gu.User.RollNumber,
                        Name = gu.User.Fullname,
                        Email = gu.User.Mail,
                        Role = gu.Role
                    })
                    .ToList(),

                ActivityLog = null
            };

            return new ApiResponse<GroupDetailDto>(200, "Lấy thành công", dto);
        }*/

        public async Task<ApiResponse<List<GroupMentorDto>>> GetExpiredGroupsBySupervisorAsync(int supervisorId)
        {
            var groups = await _groupRepository.GetExpiredGroupsByUserIdAsync(supervisorId);

            if (groups == null || !groups.Any())
                return new ApiResponse<List<GroupMentorDto>>(200, "No expired groups found.", new List<GroupMentorDto>());

            return new ApiResponse<List<GroupMentorDto>>(200, "Lấy danh sách nhóm hết hạn thành công", groups);
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
                IsExpired = g.ExpireDate != null && g.ExpireDate < DateTime.UtcNow,
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

        public async Task<string> UploadFileGroup(IFormFile file, int groupId,string semester, string? description)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new ValidationException("Not found group");

            string path = await FileUploadUtils.UploadFileAsync(file, (int)FileEnum.Group, _env,semester, "Group" + groupId);
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
                IsDownload = false,
                Description = description
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

        public async Task<object> GetMockData(int semesterId, string semesterName)
        {
            // 1️⃣ Lấy tất cả users từ mock, giữ nguyên thông tin
            var allUsers = MockData.Accounts
                .Where(a => a.User != null)
                .Select(u => new
                {
                    u.User.Fullname,
                    u.User.RollNumber,
                    u.User.Mail,
                    u.User.Phone
                })
                .ToList();

            // 2️⃣ Lấy tất cả groups của semester từ MockData
            var groups = MockData.GetGroupsForSemester(semesterId, semesterName);

            // 3️⃣ Lấy danh sách nhóm hiện có trong DB
            var dbGroups = await _groupRepository.GetAllAsync(); // giả sử trả về List<Group> có Code + SemesterId

            // 4️⃣ Kiểm tra nhóm đã tồn tại theo Code + SemesterId
            var alreadyExist = groups
                .Where(g => dbGroups.Any(db => db.Code?.ToUpper() == g.Code?.ToUpper() && db.SemesterId == g.SemesterId))
                .ToList();

            var notExistYet = groups
                .Where(g => dbGroups.All(db => db.Code?.ToUpper() != g.Code?.ToUpper() || db.SemesterId != g.SemesterId))
                .ToList();

            // 5️⃣ Lấy danh sách MajorCategories trong DB và MockData
            var dbMajors = await _majorRepository.getAllCourse();
            var mockMajors = MockData.MajorCategories;

            var majorExistingCodes = dbMajors.Select(m => m.Code?.ToUpper()).ToHashSet();
            var alreadyExistMajors = mockMajors.Where(m => majorExistingCodes.Contains(m.Code?.ToUpper())).ToList();
            var notExistMajors = mockMajors.Where(m => !majorExistingCodes.Contains(m.Code?.ToUpper())).ToList();

            // 6️⃣ Build kết quả trả về
            var result = new
            {
                MajorCategories = new
                {
                    AlreadyExist = alreadyExistMajors.Select(m => new { m.Code, m.Name }),
                    NotExistYet = notExistMajors.Select(m => new { m.Code, m.Name })
                },
                Groups = new
                {
                    AlreadyExist = alreadyExist.Select(group => new
                    {
                        GroupCode = group.Code,
                        GroupName = group.Name,
                        Profession = group.Profession,
                        VietnameseTitle = group.VietnameseTitle,
                        Description = group.Description,
                        Status = group.StatusId,
                        ExpireDate = group.ExpireDate,
                        Members = group.GroupUsers.Select(gu =>
                        {
                            var user = allUsers.FirstOrDefault(u => u.RollNumber == gu.User.RollNumber);
                            return new
                            {
                                Fullname = user?.Fullname,
                                RollNumber = user?.RollNumber,
                                Email = user?.Mail,
                                Phone = user?.Phone,
                                RoleInGroup = gu.Role,
                                IsActive = gu.IsActive,
                                Status = gu.Status
                            };
                        }).ToList()
                    }),
                    NotExistYet = notExistYet.Select(group => new
                    {
                        GroupCode = group.Code,
                        GroupName = group.Name,
                        MajorId = group.MajorId,
                        Profession = group.Profession,
                        VietnameseTitle = group.VietnameseTitle,
                        Description = group.Description,
                        Status = group.StatusId,
                        ExpireDate = group.ExpireDate,
                        Members = group.GroupUsers.Select(gu =>
                        {
                            var user = allUsers.FirstOrDefault(u => u.RollNumber == gu.User.RollNumber);
                            return new
                            {
                                Fullname = user?.Fullname,
                                RollNumber = user?.RollNumber,
                                Email = user?.Mail,
                                Phone = user?.Phone,
                                RoleInGroup = gu.Role,
                                IsActive = gu.IsActive,
                                Status = gu.Status
                            };
                        }).ToList()
                    })
                }
            };

            return result;
        }

        public async Task<Group> UpdateExpireDateAsync(int groupId, DateTime newExpireDate, string userRole)
    {
        if (userRole != "Supervisor")
            throw new UnauthorizedAccessException("Only Supervisors can update ExpireDate.");

        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null)
            throw new KeyNotFoundException($"Group with Id {groupId} not found.");

        var token = _httpContextAccessor.HttpContext?.Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
             throw new InvalidOperationException("Token not found in cookie.");

         var semesterInfo = _jwtService.GetSemesterFromToken(token);

            if (string.IsNullOrEmpty(semesterInfo.End_Time) ||
            !DateTime.TryParse(semesterInfo.End_Time, out DateTime semesterEndDate))
            {
                throw new InvalidOperationException("Semester End_Time is invalid.");
            }

            if (newExpireDate < semesterEndDate)
            {
                throw new ArgumentException(
                    $"ExpireDate must be greater than or equal to semester end date ({semesterEndDate:dd/MM/yyyy})."
                );
            }
            group.ExpireDate = newExpireDate;

        await _groupRepository.UpdateAsync(group);

        return group;
    }

        /* public async Task<object> CreateMockData(int semesterId)
         {
             // 1️⃣ Tạo / cập nhật MajorCategories
             var majorCategories = MockData.MajorCategories;
             foreach (var major in majorCategories)
             {
                 var existingMajor = await _majorRepository.FindByCodeAsync(major.Code);
                 if (existingMajor == null)
                 {
                     await _majorRepository.CreateAsync(major);
                 }
                 else
                 {
                     bool isUpdated = false;
                     if (existingMajor.IsActive != major.IsActive)
                     {
                         existingMajor.IsActive = major.IsActive;
                         isUpdated = true;
                     }
                     if (isUpdated)
                     {
                         await _majorRepository.UpdateAsync(existingMajor);
                     }
                 }
             }

             // 2️⃣ Tạo / cập nhật Accounts và Users
             var accounts = MockData.Accounts;
             var usernames = accounts.Select(a => a.Username.ToLower()).ToList();
             var existingAccounts = await _accountRepository.GetAllAsync(a => usernames.Contains(a.Username.ToLower()));

             var newAccounts = new List<Account>();
             var updatedAccounts = new List<Account>();

             foreach (var account in accounts)
             {
                 var existingAccount = existingAccounts.FirstOrDefault(e => e.Username.Equals(account.Username, StringComparison.OrdinalIgnoreCase));
                 if (existingAccount == null)
                 {
                     newAccounts.Add(account);
                 }
                 else
                 {
                     bool isAccountUpdated = false;
                     if (existingAccount.Password != account.Password) { existingAccount.Password = account.Password; isAccountUpdated = true; }
                     if (existingAccount.RoleId != account.RoleId) { existingAccount.RoleId = account.RoleId; isAccountUpdated = true; }

                     var existingUser = existingAccount.User;
                     var newUser = account.User;
                     if (existingUser != null && newUser != null)
                     {
                         if (existingUser.RollNumber != newUser.RollNumber) existingUser.RollNumber = newUser.RollNumber;
                         if (existingUser.Fullname != newUser.Fullname) existingUser.Fullname = newUser.Fullname;
                         if (existingUser.Dob != newUser.Dob) existingUser.Dob = newUser.Dob;
                         if (existingUser.Gender != newUser.Gender) existingUser.Gender = newUser.Gender;
                         if (existingUser.Mail != newUser.Mail) existingUser.Mail = newUser.Mail;
                         if (existingUser.Phone != newUser.Phone) existingUser.Phone = newUser.Phone;
                         if (existingUser.MajorId != newUser.MajorId) existingUser.MajorId = newUser.MajorId;
                         if (existingUser.CampusId != newUser.CampusId) existingUser.CampusId = newUser.CampusId;
                         if (existingUser.CapstoneProject != newUser.CapstoneProject) existingUser.CapstoneProject = newUser.CapstoneProject;
                         if (existingUser.Address != newUser.Address) existingUser.Address = newUser.Address;
                         if (existingUser.StatusId != newUser.StatusId) existingUser.StatusId = newUser.StatusId;
                     }

                     if (isAccountUpdated || existingUser != null) updatedAccounts.Add(existingAccount);
                 }
             }

             if (newAccounts.Any())
                 await _accountRepository.CreateUsers(newAccounts);

             foreach (var updatedAccount in updatedAccounts)
                 await _accountRepository.UpdateAsync(updatedAccount);

             var allAccounts = existingAccounts.Concat(newAccounts).ToList();

             // 3️⃣ Lấy groups từ MockData theo semesterId
             var groups = MockData.GetGroupsForSemester(semesterId);

             // 4️⃣ Lấy nhóm hiện có trong DB theo Code + SemesterId
             var existingGroups = await _groupRepository.GetAllAsync(g =>
                 groups.Select(gr => gr.Code).Contains(g.Code) && g.SemesterId == semesterId);

             foreach (var group in groups)
             {
                 var existingGroup = existingGroups.FirstOrDefault(g => g.Code == group.Code && g.SemesterId == semesterId);
                 var semester = await _semesterRepository.GetSemesterByIdAsync(group.SemesterId ?? 0);
                 group.ExpireDate = semester?.EndAt;

                 if (existingGroup == null)
                 {
                     await _groupRepository.CreateGroups(new List<Group> { group });
                 }
                 else
                 {
                     bool isGroupUpdated = false;
                     if (existingGroup.Name != group.Name) { existingGroup.Name = group.Name; isGroupUpdated = true; }
                     if (existingGroup.Profession != group.Profession) { existingGroup.Profession = group.Profession; isGroupUpdated = true; }
                     if (existingGroup.MajorId != group.MajorId) { existingGroup.MajorId = group.MajorId; isGroupUpdated = true; }
                     if (existingGroup.Description != group.Description) { existingGroup.Description = group.Description; isGroupUpdated = true; }
                     if (existingGroup.VietnameseTitle != group.VietnameseTitle) { existingGroup.VietnameseTitle = group.VietnameseTitle; isGroupUpdated = true; }
                     if (existingGroup.StatusId != group.StatusId) { existingGroup.StatusId = group.StatusId; isGroupUpdated = true; }
                     if (existingGroup.ExpireDate != semester?.EndAt) { existingGroup.ExpireDate = semester?.EndAt; isGroupUpdated = true; }

                     foreach (var newGU in group.GroupUsers)
                     {
                         var existingGU = existingGroup.GroupUsers.FirstOrDefault(gu => gu.UserId == newGU.UserId);
                         if (existingGU == null)
                         {
                             existingGroup.GroupUsers.Add(newGU);
                             isGroupUpdated = true;
                         }
                         else
                         {
                             if (existingGU.Role != newGU.Role) { existingGU.Role = newGU.Role; isGroupUpdated = true; }
                             if (existingGU.IsActive != newGU.IsActive) { existingGU.IsActive = newGU.IsActive; isGroupUpdated = true; }
                             if (existingGU.Status != newGU.Status) { existingGU.Status = newGU.Status; isGroupUpdated = true; }
                             existingGU.UpdateAt = DateTime.Now;
                         }
                     }

                     if (isGroupUpdated)
                         await _groupRepository.UpdateAsync(existingGroup);
                 }
             }

             return new
             {
                 Message = $"Create mock data successfully for semester {semesterId}"
             };
         }*/

        public async Task<object> CreateMockData(int semesterId, string semesterName)
        {
            // 1️⃣ Tạo / cập nhật MajorCategories
            var majorCategories = MockData.MajorCategories;
            foreach (var major in majorCategories)
            {
                var existingMajor = await _majorRepository.FindByCodeAsync(major.Code);
                if (existingMajor == null)
                {
                    await _majorRepository.CreateAsync(new MajorCategory
                    {
                        Code = major.Code,
                        Name = major.Name,
                        IsActive = major.IsActive
                    });
                }
                else
                {
                    bool isUpdated = false;
                    if (existingMajor.IsActive != major.IsActive)
                    {
                        existingMajor.IsActive = major.IsActive;
                        isUpdated = true;
                    }
                    if (isUpdated)
                        await _majorRepository.UpdateAsync(existingMajor);
                }
            }

            // 2️⃣ Tạo / cập nhật Accounts và Users
            var accounts = MockData.Accounts;
            var usernames = accounts.Select(a => a.Username.ToLower()).ToList();
            var existingAccounts = await _accountRepository.GetAllAsync(a => usernames.Contains(a.Username.ToLower()));

            var newAccounts = new List<Account>();

            foreach (var account in accounts)
            {
                var existingAccount = existingAccounts
                    .FirstOrDefault(e => e.Username.Equals(account.Username, StringComparison.OrdinalIgnoreCase));

                if (existingAccount == null)
                {
                    newAccounts.Add(account); // Chưa có → tạo mới
                }
                else
                {
                    // Đã có → update thông tin account nếu cần
                    bool isAccountUpdated = false;
/*                    if (existingAccount.Password != account.Password)
                    {
                        existingAccount.Password = account.Password;
                        isAccountUpdated = true;
                    }*/
                    if (existingAccount.RoleId != account.RoleId)
                    {
                        existingAccount.RoleId = account.RoleId;
                        isAccountUpdated = true;
                    }

                    var existingUser = existingAccount.User;
                    var newUser = account.User;
                    if (existingUser != null && newUser != null)
                    {
                        if (existingUser.RollNumber != newUser.RollNumber) existingUser.RollNumber = newUser.RollNumber;
                        if (existingUser.Fullname != newUser.Fullname) existingUser.Fullname = newUser.Fullname;
                        if (existingUser.Dob != newUser.Dob) existingUser.Dob = newUser.Dob;
                        if (existingUser.Gender != newUser.Gender) existingUser.Gender = newUser.Gender;
                        if (existingUser.Mail != newUser.Mail) existingUser.Mail = newUser.Mail;
                        if (existingUser.Phone != newUser.Phone) existingUser.Phone = newUser.Phone;
                        if (existingUser.MajorId != newUser.MajorId) existingUser.MajorId = newUser.MajorId;
                        if (existingUser.CampusId != newUser.CampusId) existingUser.CampusId = newUser.CampusId;
                        if (existingUser.CapstoneProject != newUser.CapstoneProject) existingUser.CapstoneProject = newUser.CapstoneProject;
                        if (existingUser.Address != newUser.Address) existingUser.Address = newUser.Address;
                        if (existingUser.StatusId != newUser.StatusId) existingUser.StatusId = newUser.StatusId;
                    }

                    if (isAccountUpdated)
                        await _accountRepository.UpdateAsync(existingAccount);
                }
            }

            if (newAccounts.Any())
                await _accountRepository.CreateUsers(newAccounts);

            // 3️⃣ Lấy tất cả accounts mới nhất để gán vào GroupUser
            var allAccounts = await _accountRepository.GetAllAsync(a => usernames.Contains(a.Username.ToLower()));

            // 4️⃣ Tạo groups
            var groups = MockData.GetGroupsForSemester(semesterId, semesterName);

            var existingGroups = await _groupRepository.GetAllAsync(g =>
                groups.Select(gr => gr.Code).Contains(g.Code) && g.SemesterId == semesterId);

            foreach (var group in groups)
            {
                var existingGroup = existingGroups.FirstOrDefault(g => g.Code == group.Code && g.SemesterId == semesterId);
                var semester = await _semesterRepository.GetSemesterByIdAsync(group.SemesterId ?? 0);
                group.ExpireDate = semester?.EndAt;

                if (existingGroup == null)
                {
                    // Clone GroupUsers: nếu User đã tồn tại thì chỉ gán UserId
                    var finalGroupUsers = new List<GroupUser>();
                    foreach (var gu in group.GroupUsers)
                    {
                        var userInDb = allAccounts.FirstOrDefault(a => a.User.RollNumber == gu.User.RollNumber)?.User;
                        if (userInDb == null) continue; // bỏ qua nếu user chưa tồn tại (nên ít khi xảy ra)

                        finalGroupUsers.Add(new GroupUser
                        {
                            UserId = userInDb.Id,
                            Role = gu.Role,
                            IsActive = gu.IsActive,
                            Status = gu.Status,
                            CreateAt = gu.CreateAt,
                            UpdateAt = gu.UpdateAt
                        });
                    }
                    group.GroupUsers = finalGroupUsers;

                    await _groupRepository.CreateGroups(new List<Group> { group });
                }
                else
                {
                    // Update group
                    bool isGroupUpdated = false;
                    if (existingGroup.Name != group.Name) { existingGroup.Name = group.Name; isGroupUpdated = true; }
                    if (existingGroup.Profession != group.Profession) { existingGroup.Profession = group.Profession; isGroupUpdated = true; }
                    if (existingGroup.MajorId != group.MajorId) { existingGroup.MajorId = group.MajorId; isGroupUpdated = true; }
                    if (existingGroup.Description != group.Description) { existingGroup.Description = group.Description; isGroupUpdated = true; }
                    if (existingGroup.VietnameseTitle != group.VietnameseTitle) { existingGroup.VietnameseTitle = group.VietnameseTitle; isGroupUpdated = true; }
                    if (existingGroup.StatusId != group.StatusId) { existingGroup.StatusId = group.StatusId; isGroupUpdated = true; }
                    if (existingGroup.ExpireDate != semester?.EndAt) { existingGroup.ExpireDate = semester?.EndAt; isGroupUpdated = true; }

                    foreach (var newGU in group.GroupUsers)
                    {
                        var userInDb = allAccounts.FirstOrDefault(a => a.User.RollNumber == newGU.User.RollNumber)?.User;
                        if (userInDb == null) continue;

                        var existingGU = existingGroup.GroupUsers.FirstOrDefault(gu => gu.UserId == userInDb.Id);
                        if (existingGU == null)
                        {
                            existingGroup.GroupUsers.Add(new GroupUser
                            {
                                UserId = userInDb.Id,
                                Role = newGU.Role,
                                IsActive = newGU.IsActive,
                                Status = newGU.Status,
                                CreateAt = newGU.CreateAt,
                                UpdateAt = newGU.UpdateAt
                            });
                            isGroupUpdated = true;
                        }
                        else
                        {
                            if (existingGU.Role != newGU.Role) { existingGU.Role = newGU.Role; isGroupUpdated = true; }
                            if (existingGU.IsActive != newGU.IsActive) { existingGU.IsActive = newGU.IsActive; isGroupUpdated = true; }
                            if (existingGU.Status != newGU.Status) { existingGU.Status = newGU.Status; isGroupUpdated = true; }
                            existingGU.UpdateAt = DateTime.Now;
                        }
                    }

                    if (isGroupUpdated)
                        await _groupRepository.UpdateAsync(existingGroup);
                }
            }

            return new
            {
                Message = $"Create mock data successfully for semester {semesterName}"
            };
        }
    }

}
