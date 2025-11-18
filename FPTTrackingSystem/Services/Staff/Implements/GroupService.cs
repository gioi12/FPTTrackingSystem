using Azure;
using DataTranferObjects.Common.Response;
using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Request;
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

        public GroupService(IGroupRepository groupRepository,IMajorRepository majorRepository, ISemesterService semesterService,  AuthUtils authUtils, IWebHostEnvironment env,IAttachmentRepository attachmentRepository, IHttpContextAccessor httpContextAccessor,IAccountRepository accountRepository,ISemesterRepository semesterRepository)
        {
            _groupRepository = groupRepository;
            _authUtils = authUtils;
            _env = env;
            _attachmentRepository = attachmentRepository;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
            _semesterRepository = semesterRepository;
            _majorRepository = majorRepository;
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


        /*  public async Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize)
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
                          Data = new PagedData<GroupDto> { Items = new List<GroupDto>(), Total = 0 }
                      };
                  }

                  var userGroupIds = user.Groups.ToList();
                  query = query.Where(g => userGroupIds.Contains(int.Parse(g.Id)));
              }

              var total = await query.CountAsync();

              // ✅ Lúc này EF chỉ lấy đúng page bạn cần
              var groups = await query
                  .OrderBy(g => g.Id)
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

              // ✅ Convert Supervisor sang List<string> nếu EF trả về IEnumerable
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
          }*/


        /*public async Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _groupRepository.GetGroupsQuery().AsNoTracking();
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
                     CourseCode = g.Major.Code,
                     GroupCode = g.Code,
                     Term = g.Semester != null ? g.Semester.Name : "",
                     Major = g.Major != null ? g.Major.Name : "",
                     StudentCount = g.GroupUsers.Count(gu => gu.Role == RoleEnum.Student.ToString() || gu.Role == "Leader" || gu.Role == "Secretary"),
                     Supervisor = g.GroupUsers
                        .Where(gu => gu.Role == "Supervisor" || gu.Role == "SuperviorHead")
                        .Select(gu => gu.User.Fullname)
                        .ToList(),
                     SubmittedDocs = false,
                 }).AsNoTracking()
                  .ToListAsync();

            return new PagedResponse<GroupDto>
            {
                Status = 200,
                Message = "Lấy thành công",
                Data = new PagedData<GroupDto>
                {
                    Items = groups,
                    Total = 0
                }
            };
        }*/

        public async Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int id)
        {
            // Lấy thông tin user từ JWT
            var user = await _authUtils.GetUserInfoFromCookie();

            // Lấy group theo ID
            var group = await _groupRepository.GetByIdAsync(id);

            if (group == null)
            {
                return new ApiResponse<GroupDetailDto>(200, "Group not found.", null);
            }

            // STAFF: bỏ toàn bộ check kỳ + check thuộc nhóm
            if (user.Role != RoleEnum.Staff.ToString())
            {
                // --- CHECK KỲ ---
                int? semesterIdValue = user.SemesterId;

                if (semesterIdValue == null || semesterIdValue == 0)
                {
                    return new ApiResponse<GroupDetailDto>(400, "Current semester information not found.", null);
                }

                int currentSemesterId = semesterIdValue.Value;

                if (group.SemesterId != currentSemesterId)
                {
                    return new ApiResponse<GroupDetailDto>(200, "This group does not belong to the current semester.", null);
                }

                // --- CHECK THÀNH VIÊN NHÓM ---
                if (user.Role == "Student" || user.Role == "Supervisor" || user.Role == "SupervisorHead")
                {
                    if (user.Groups == null || !user.Groups.Contains(id))
                    {
                        return new ApiResponse<GroupDetailDto>(403, "Bạn không có quyền truy cập nhóm này.", new GroupDetailDto());
                    }
                }
            }

            // Map DTO
            var dto = new GroupDetailDto
            {
                Id = group.Id.ToString(),
                ProjectName = group.Name,
                GroupCode = group.Code,
                SemesterId = group.SemesterId,

                Supervisors = group.GroupUsers
                    .Where(gu => gu.User != null && (gu.Role == "Supervisor" || gu.Role == "SupervisorHead"))
                    .Select(gu => gu.User.Fullname)
                    .ToList(),

                SupervisorsInfor = group.GroupUsers
                    .Where(gu => gu.User != null && (gu.Role == "Supervisor" || gu.Role == "SupervisorHead"))
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

/*        public static List<Group> GetGroups(
            int semesterId,
            int user1Id, int user2Id, int user3Id, int user4Id, int user5Id, int mentor1Id,
            int user6Id, int user7Id, int user8Id, int user9Id, int user10Id, int mentor2Id,
            params int[] mentorIds // mentor3 → mentor52
        )
        {
            var groups = new List<Group>();

            // ==============================
            // 1️⃣ GIỮ NGUYÊN GROUP 1
            // ==============================
            groups.Add(new Group
            {
                Code = "G01",
                Name = "Capstone Team Alpha",
                SemesterId = semesterId,
                Profession = "Software Engineering",
                MajorId = 1,
                Description = "G01 description",
                VietnameseTitle = "Nhóm 1",
                StatusId = "ACTIVE",
                CreateAt = DateTime.Now.AddMonths(-1),

                GroupUsers = new List<GroupUser>
        {
            new GroupUser { UserId = user1Id, Role = "Student", IsActive = true, Status = "ACTIVE", CreateAt = DateTime.Now, UpdateAt = DateTime.Now },
            new GroupUser { UserId = user2Id, Role = "Student", IsActive = true, Status = "ACTIVE", CreateAt = DateTime.Now, UpdateAt = DateTime.Now },
            new GroupUser { UserId = user3Id, Role = "Student", IsActive = true, Status = "ACTIVE", CreateAt = DateTime.Now, UpdateAt = DateTime.Now },
            new GroupUser { UserId = user4Id, Role = "Student", IsActive = true, Status = "ACTIVE", CreateAt = DateTime.Now, UpdateAt = DateTime.Now },
            new GroupUser { UserId = user5Id, Role = "Student", IsActive = true, Status = "ACTIVE", CreateAt = DateTime.Now, UpdateAt = DateTime.Now },

            new GroupUser { UserId = mentor1Id, Role = "Mentor", IsActive = true, Status = "ACTIVE", CreateAt = DateTime.Now, UpdateAt = DateTime.Now }
        }
            });

            // ==============================
            // 2️⃣ GIỮ NGUYÊN GROUP 2
            // ==============================
            groups.Add(new Group
            {
                Code = "G02",
                Name = "Capstone Team Beta",
                SemesterId = semesterId,
                Profession = "Software Engineering",
                MajorId = 1,
                Description = "G02 description",
                VietnameseTitle = "Nhóm 2",
                StatusId = "ACTIVE",
                CreateAt = DateTime.Now.AddMonths(-1),

                GroupUsers = new List<GroupUser>
        {
            new GroupUser { UserId = user6Id, Role = "Student", IsActive = true, Status = "ACTIVE" },
            new GroupUser { UserId = user7Id, Role = "Student", IsActive = true, Status = "ACTIVE" },
            new GroupUser { UserId = user8Id, Role = "Student", IsActive = true, Status = "ACTIVE" },
            new GroupUser { UserId = user9Id, Role = "Student", IsActive = true, Status = "ACTIVE" },
            new GroupUser { UserId = user10Id, Role = "Student", IsActive = true, Status = "ACTIVE" },

            new GroupUser { UserId = mentor2Id, Role = "Mentor", IsActive = true, Status = "ACTIVE" }
        }
            });

            // ==============================
            // 3️⃣ TẠO 50 GROUP TỰ ĐỘNG: G03 → G52
            // ==============================

            int totalAutoGroups = 50;              // số nhóm cần tạo
            int startIndex = 3;                    // bắt đầu từ G03
            int endIndex = startIndex + totalAutoGroups - 1; // G52

            for (int i = startIndex; i <= endIndex; i++)
            {
                int mentorIndex = (i - 3);   // mentorIds[0] ứng với G03

                if (mentorIndex >= mentorIds.Length)
                    break;

                groups.Add(new Group
                {
                    Code = $"G{i:00}",
                    Name = $"Capstone Team {i:00}",
                    SemesterId = semesterId,
                    Profession = "Software Engineering",
                    MajorId = 1,
                    Description = $"Auto generated group number {i}",
                    VietnameseTitle = $"Nhóm {i}",
                    StatusId = "ACTIVE",
                    CreateAt = DateTime.Now.AddMonths(-1),

                    GroupUsers = new List<GroupUser>
            {
                new GroupUser
                {
                    UserId = mentorIds[mentorIndex],
                    Role = "Mentor",
                    IsActive = true,
                    Status = "ACTIVE",
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                }
            }
                });
            }

            return groups;
        }*/

        public async Task<object> GetMockData()
        {
            var mentorsArray = Enumerable.Range(13, 50).ToArray();
/*            var userIdMap = new Dictionary<string, int>
    {
        {"SE140001", 1},
        {"SE140002", 2},
        {"SE140003", 3},
        {"SE140004", 4},
        {"SE140005", 5},
        {"ME01", 6},
        {"SE140006", 7},
        {"SE140007", 8},
        {"SE140008", 9},
        {"SE140009", 10},
        {"SE140010", 11},
        {"ME03", 12}
    };*/

            // 1️⃣ Map RollNumber -> UserId giả
            var userIdMap = new Dictionary<string, int>
    {
        {"SE150001", 1},
        {"SE150002", 2},
        {"SE150003", 3},
        {"SE150004", 4},
        {"SE150005", 5},
        {"ME01", 6},
        {"SE150006", 7},
        {"SE150007", 8},
        {"SE150008", 9},
        {"SE150009", 10},
        {"SE150010", 11},
        {"ME03", 12}
    };

            // 2️⃣ Lấy tất cả users
            var allUsers = MockData.Accounts
                .Where(a => a.User != null)
                .Select(u => new
                {
                    Id = userIdMap.ContainsKey(u.User.RollNumber) ? userIdMap[u.User.RollNumber] : 0,
                    u.User.Fullname,
                    u.User.RollNumber,
                    u.User.Mail,
                    u.User.Phone
                })
                .ToList();

            // 3️⃣ Tạo groups với Id kỳ học thật
            var groups = MockData.GetGroups(
                8,
                1, 2, 3, 4, 5, 6,
                7, 8, 9, 10, 11, 12
            );

            // 4️⃣ Lấy danh sách group hiện có trong DB
            var dbGroups = await _groupRepository.GetAllAsync(); // giả sử repo trả về List<Group> với Code

            var existingCodes = dbGroups.Select(g => g.Code?.ToUpper()).ToHashSet();

            var alreadyExist = groups.Where(g => existingCodes.Contains(g.Code?.ToUpper())).ToList();
            var notExistYet = groups.Where(g => !existingCodes.Contains(g.Code?.ToUpper())).ToList();

            // 5️⃣ Lấy danh sách MajorCategory thật trong DB
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

        group.ExpireDate = newExpireDate;

        await _groupRepository.UpdateAsync(group);

        return group;
    }

        /*public async Task<object> CreateMockData()
        {

            // 🔹 2. Tạo MajorCategory nếu chưa tồn tại
            var majorCategories = MockData.MajorCategories;
            foreach (var major in majorCategories)
            {
                var existingMajor = await _majorRepository.FindByCodeAsync(major.Code);

                if (existingMajor == null)
                {
                    await _majorRepository.CreateAsync(major);
                }
                else if (existingMajor.IsActive != null)
                {
                    existingMajor.IsActive = true;
                    await _majorRepository.UpdateAsync(existingMajor);
                }
            }

            // 🔹 3. Xử lý accounts
            var accounts = MockData.Accounts;
            var mockUsernames = accounts.Select(a => a.Username.ToLower()).ToList();

            // Lấy các account đã tồn tại
            var existingAccounts = await _accountRepository.GetAllAsync(
                a => mockUsernames.Contains(a.Username.ToLower())
            );

            // Chỉ tạo user mới
            var newAccounts = accounts
                .Where(a => !existingAccounts.Any(e => e.Username.Equals(a.Username, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (newAccounts.Any())
            {
                await _accountRepository.CreateUsers(newAccounts);
            }

            // Dùng tất cả accounts (cũ + mới)
            var allAccounts = existingAccounts.Concat(newAccounts).ToList();

            // Map users cho tạo group
            int user1Id = allAccounts.First(a => a.Username == "gioidmhe171512@fpt.edu.vn").Users.First().Id;
            int user2Id = allAccounts.First(a => a.Username == "haildhe172452@fpt.edu.vn").Users.First().Id;
            int user3Id = allAccounts.First(a => a.Username == "cuonghvhe176362@fpt.edu.vn").Users.First().Id;
            int user4Id = allAccounts.First(a => a.Username == "handghe170064@fpt.edu.vn").Users.First().Id;
            int user5Id = allAccounts.First(a => a.Username == "huongtt170064@fpt.edu.vn").Users.First().Id;
            int mentor1Id = allAccounts.First(a => a.Username == "lampt2@gmail.com").Users.First().Id;

            // 5️⃣ Lấy nhóm từ MockData
            var groups = MockData.GetGroups(
                1,
                user1Id, user2Id, user3Id, user4Id, user5Id,
                mentor1Id
            );

            // 6️⃣ Tạo nhóm
            await _groupRepository.CreateGroups(groups);

            return "Create mock data successfully";
        }*/
        public async Task<object> CreateMockData()
        {
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

            // User IDs for G01
            int user1Id = allAccounts.First(a => a.Username == "gioidmhe171512@fpt.edu.vn").User.Id;
            int user2Id = allAccounts.First(a => a.Username == "haildhe172452@fpt.edu.vn").User.Id;
            int user3Id = allAccounts.First(a => a.Username == "cuonghvhe176362@fpt.edu.vn").User.Id;
            int user4Id = allAccounts.First(a => a.Username == "handghe170064@fpt.edu.vn").User.Id;
            int user5Id = allAccounts.First(a => a.Username == "huongtt170064@fpt.edu.vn").User.Id;
            int mentor1Id = allAccounts.First(a => a.Username == "lampt2@gmail.com").User.Id;

            // User IDs for G02
            int user6Id = allAccounts.First(a => a.Username == "namnthe172123@fpt.edu.vn").User.Id;
            int user7Id = allAccounts.First(a => a.Username == "minhpthe171234@fpt.edu.vn").User.Id;
            int user8Id = allAccounts.First(a => a.Username == "anhtthe173456@fpt.edu.vn").User.Id;
            int user9Id = allAccounts.First(a => a.Username == "quangnmhe175678@fpt.edu.vn").User.Id;
            int user10Id = allAccounts.First(a => a.Username == "linhnthe176789@fpt.edu.vn").User.Id;
            int mentor2Id = allAccounts.First(a => a.Username == "thanhbv@gmail.com").User.Id;

            /*            int user1Id = allAccounts.First(a => a.Username == "gioidmhe171512").User.Id;
                        int user2Id = allAccounts.First(a => a.Username == "haildhe172452").User.Id;
                        int user3Id = allAccounts.First(a => a.Username == "cuonghvhe176362").User.Id;
                        int user4Id = allAccounts.First(a => a.Username == "handghe170064").User.Id;
                        int user5Id = allAccounts.First(a => a.Username == "huongtt170064").User.Id;
                        int mentor1Id = allAccounts.First(a => a.Username == "lampt2@gmail.com").User.Id;

                        int user6Id = allAccounts.First(a => a.Username == "namnthe172123").User.Id;
                        int user7Id = allAccounts.First(a => a.Username == "minhpthe171234").User.Id;
                        int user8Id = allAccounts.First(a => a.Username == "anhtthe173456").User.Id;
                        int user9Id = allAccounts.First(a => a.Username == "quangnmhe175678").User.Id;
                        int user10Id = allAccounts.First(a => a.Username == "linhnthe176789").User.Id;
                        int mentor2Id = allAccounts.First(a => a.Username == "thanhbv@gmail.com").User.Id;*/



            // Lấy tất cả groups
            var groups = MockData.GetGroups(
                8,
                user1Id, user2Id, user3Id, user4Id, user5Id, mentor1Id,
                user6Id, user7Id, user8Id, user9Id, user10Id, mentor2Id
            );

            var existingGroups = await _groupRepository.GetAllAsync(g => groups.Select(gr => gr.Code).Contains(g.Code));

            foreach (var group in groups)
            {
                var existingGroup = existingGroups.FirstOrDefault(g => g.Code == group.Code);
                if (existingGroup == null)
                {
                    var semester = await _semesterRepository.GetSemesterByIdAsync(group.SemesterId ?? 0);
                    group.ExpireDate = semester.EndAt;
                    await _groupRepository.CreateGroups(new List<Group> { group });
                }
                else
                {

                    bool isGroupUpdated = false;
                    if (existingGroup.Name != group.Name) { existingGroup.Name = group.Name; isGroupUpdated = true; }
                    if (existingGroup.SemesterId != group.SemesterId) { existingGroup.SemesterId = group.SemesterId; isGroupUpdated = true; }
                    if (existingGroup.Profession != group.Profession) { existingGroup.Profession = group.Profession; isGroupUpdated = true; }
                    if (existingGroup.MajorId != group.MajorId) { existingGroup.MajorId = group.MajorId; isGroupUpdated = true; }
                    if (existingGroup.Description != group.Description) { existingGroup.Description = group.Description; isGroupUpdated = true; }
                    if (existingGroup.VietnameseTitle != group.VietnameseTitle) { existingGroup.VietnameseTitle = group.VietnameseTitle; isGroupUpdated = true; }
                    if (existingGroup.StatusId != group.StatusId) { existingGroup.StatusId = group.StatusId; isGroupUpdated = true; }
                    if (existingGroup.SemesterId != group.SemesterId)
                    {
                        existingGroup.SemesterId = group.SemesterId;
                        isGroupUpdated = true;
                    }

                    var semester = await _semesterRepository.GetSemesterByIdAsync(existingGroup.SemesterId ?? 0);

                    if (existingGroup.ExpireDate != semester?.EndAt)
                    {
                        existingGroup.ExpireDate = semester?.EndAt;
                        isGroupUpdated = true;
                    }
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
                        existingGroup.ExpireDate = semester?.EndAt;
                    await _groupRepository.UpdateAsync(existingGroup);
                }
            }

            // Lấy riêng G02 nếu cần
            var groupG02 = groups.FirstOrDefault(g => g.Code == "G02");

            var userDict = allAccounts.ToDictionary(a => a.User.Id, a => a.User);

            var groupG02Dto = new
            {
                GroupCode = groupG02?.Code,
                GroupName = groupG02?.Name,
                MajorId = groupG02?.MajorId,
                Profession = groupG02?.Profession,
                VietnameseTitle = groupG02?.VietnameseTitle,
                Description = groupG02?.Description,
                Status = groupG02?.StatusId,
                Members = groupG02?.GroupUsers.Select(gu =>
                {
                    userDict.TryGetValue(gu.UserId, out var user);
                    return new
                    {
                        UserId = gu.UserId,
                        Fullname = user?.Fullname,
                        RollNumber = user?.RollNumber,
                        Email = user?.Mail,
                        Phone = user?.Phone,
                        RoleInGroup = gu.Role,
                        IsActive = gu.IsActive,
                        Status = gu.Status
                    };
                }).ToList()
            };

            return new
            {
                Message = "Create mock data successfully",
                GroupG02 = groupG02Dto
            };
        }

    }

}
