using Azure.Core;
using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using DataTranferObjects.Staff.Semester;
using Entities.Models;
using FPTTrackingSystem.Helper;
using FPTTrackingSystem.Hepler;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Interfaces;
using System.Globalization;

namespace FPTTrackingSystem.Services.Staff.Implementations
{
    public class SemesterService : ISemesterService
    {
        private readonly FpttrackingSystemContext _context;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly ILogService _logService;
        private readonly AuthUtils _authUtils;
        private readonly ILogger<SemesterService> _logger;
        public SemesterService(ISemesterRepository semesterRepository, IMajorRepository majorRepositoy, FpttrackingSystemContext context, ILogService logService, AuthUtils authUtils, ILogger<SemesterService> logger)
        {
            _semesterRepository = semesterRepository;
            _majorRepository = majorRepositoy;
            _context = context;
            _logService = logService;
            _authUtils = authUtils;
            _logger = logger;
        }

        public async Task<ApiResponse<SemesterActiveRes>> GetSemesterActiveAndMajors()
        {
            var semester = await _semesterRepository.findActive();
            var majors = await _majorRepository.findAll();
            SemesterActiveRes se = new SemesterActiveRes()
            {
                Id = semester.Id,
                Name = semester.Name,
                Description = semester.Description,
                majors = majors.Select(x => new
                {
                    x.Id,
                    x.Name,
                }).Cast<object>().ToList()
            };
            return ApiResponse<SemesterActiveRes>.Success(se);
        }

        /* public async Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest request)
         {
             if (string.IsNullOrWhiteSpace(request.Name))
                 throw new ArgumentException("Semester name cannot be empty.");

             if (request.Name.Length > 100)
                 throw new ArgumentException("Semester name cannot exceed 100 characters.");

             if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > 500)
                 throw new ArgumentException("Description cannot exceed 500 characters.");

             // ✅ Get current user from cookie
             var user = await _authUtils.GetUserInfoFromCookie();
             if (user == null)
                 throw new UnauthorizedAccessException("User authentication failed.");

             if (!string.Equals(user.Role, RoleEnum.Staff.ToString(), StringComparison.OrdinalIgnoreCase))
                 throw new UnauthorizedAccessException("Only Staff members can create semesters.");

             // ✅ Validate logical date range
             var mockSemester = MockData.AllSemesters
                 .FirstOrDefault(s => string.Equals(s.Name.Trim(), request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

             DateTime? startAt = null;
             DateTime? endAt = null;

             if (mockSemester != null)
             {
                 startAt = mockSemester.StartAt;
                 endAt = mockSemester.EndAt;
             }

             // ✅ Disable any currently active semester
             var activeSemester = await _context.Semesters.FirstOrDefaultAsync(s => s.IsActive == true);
             if (activeSemester != null)
             {
                 activeSemester.IsActive = false;
                 _context.Semesters.Update(activeSemester);
             }

             // ✅ Create new semester
             var semester = new Semester
             {
                 Name = request.Name.Trim(),
                 Description = request.Description?.Trim(),
                 StartAt = startAt,
                 EndAt = endAt,
                 IsActive = true
             };

             try
             {
                 await _context.Semesters.AddAsync(semester);
                 await _context.SaveChangesAsync();
             }
             catch (DbUpdateException dbEx)
             {
                 var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                 _logger.LogError(dbEx, "Database error while saving semester: {Message}", innerMessage);
                 throw new Exception($"Database error occurred while saving semester: {innerMessage}");
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Unexpected error while saving semester");
                 throw new Exception($"Unexpected error occurred while saving semester: {ex.Message}");
             }

             // 7️⃣ Log semester creation
             await _logService.AddLogAsync(new Log
             {
                 Name = "Create new semester",
                 EntityName = "Semester",
                 EntityId = semester.Id,
                 Action = "CREATE",
                 Description = $"Created semester '{semester.Name}' from {semester.StartAt:yyyy-MM-dd} to {semester.EndAt:yyyy-MM-dd}.",
                 UserId = user.Id ?? 0,
                 CreateAt = DateTime.Now
             });

             DateOnly startDateOnly = startAt.HasValue
     ? DateOnly.FromDateTime(startAt.Value)
     : default;

             DateOnly endDateOnly = endAt.HasValue
                 ? DateOnly.FromDateTime(endAt.Value)
                 : default;

             // 8️⃣ Generate semester weeks
             var weeks = SemesterHelper.GetWeeks(startDateOnly, endDateOnly, semester.Id);
             int learnWeekCount = 0;
             foreach (var w in weeks)
             {
                 if (w.IsVacation != null)
                 {
                     learnWeekCount++;
                     w.WeekLearn = learnWeekCount;
                 }
             }

             var semesterWeeks = weeks.Select(w => new SemesterWeek
             {
                 SemesterId = semester.Id,
                 WeekNumber = w.WeekNumber,
                 StartAt = w.StartAt,
                 EndAt = w.EndAt,
                 StartAtLunar = SafeConvertToLunar(w.StartAt ?? DateTime.Now),
                 EndAtLunar = SafeConvertToLunar(w.EndAt ?? DateTime.Now),
                 IsVacation = w.IsVacation,
                 WeekLearn = w.WeekLearn
             }).ToList();

             await _context.SemesterWeeks.AddRangeAsync(semesterWeeks);
             await _context.SaveChangesAsync();

             // 9️⃣ Get all active milestones
             var activeMilestones = await _context.Milestones
                 .Include(m => m.MilestoneItems)
                 .Where(m => m.IsActive == true)
                 .ToListAsync();

             // 🔟 Create Deliverables and DeliveryItems from active milestones
             var deliverables = new List<Deliverable>();
             var deliveryItems = new List<DeliveryItem>();

             foreach (var milestone in activeMilestones)
             {
                 var deliverable = new Deliverable
                 {
                     MilestoneId = milestone.Id,
                     SemesterId = semester.Id,
                     Name = milestone.Name,
                     Description = milestone.Description,
                     Deadline = milestone.Deadline,
                     IsActive = true,
                     MajorId = milestone.MajorId
                 };
                 deliverables.Add(deliverable);

                 foreach (var item in milestone.MilestoneItems)
                 {
                     var deliveryItem = new DeliveryItem
                     {
                         Name = item.Name,
                         Description = item.Description,
                         MilestoneItemId = item.Id,
                         Deliverable = deliverable
                     };
                     deliveryItems.Add(deliveryItem);
                 }
             }

             await _context.Deliverables.AddRangeAsync(deliverables);
             await _context.DeliveryItems.AddRangeAsync(deliveryItems);
             await _context.SaveChangesAsync();

             // 11️⃣ Log milestone cloning
             await _logService.AddLogAsync(new Log
             {
                 Name = "Clone active milestones to deliverables",
                 EntityName = "Deliverable",
                 Action = "CREATE",
                 Description = $"Automatically generated {deliverables.Count} Deliverables and {deliveryItems.Count} DeliveryItems from active milestones for semester '{semester.Name}'.",
                 UserId = user.Id ?? 0,
                 CreateAt = DateTime.Now
             });

             // ✅ 12️⃣ Return response
             return new SemesterDTO
             {
                 IsActive = semester.IsActive,
                 Name = semester.Name ?? string.Empty,
                 StartAt = semester.StartAt ?? default,
                 EndAt = semester.EndAt ?? default,
                 Description = semester.Description,
                 Weeks = weeks,
             };
         }*/
        public async Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Semester name cannot be empty.");

            if (request.Name.Length > 100)
                throw new ArgumentException("Semester name cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(request.Description) &&
                request.Description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.");

            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                throw new UnauthorizedAccessException("User authentication failed.");

            if (!user.Role.Equals(RoleEnum.Staff.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Only Staff members can create semesters.");

            string name = request.Name.Trim();

            bool exists = await _context.Semesters
                .AnyAsync(s => s.Name.Trim().ToLower() == name.ToLower());

            if (exists)
                throw new ArgumentException($"Semester '{name}' already exists.");

            var mockSemester = MockData.AllSemesters
                .FirstOrDefault(s =>
                    s.Name.Trim().Equals(name, StringComparison.OrdinalIgnoreCase));

            DateTime? startAt = mockSemester?.StartAt;
            DateTime? endAt = mockSemester?.EndAt;
            bool? isActive = mockSemester?.IsActive;

            var semester = new Semester
            {
                Name = name,
                Description = request.Description?.Trim(),
                StartAt = startAt,
                EndAt = endAt,
                IsActive = isActive
            };

            await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(new Log
            {
                Name = "Create new semester",
                EntityName = "Semester",
                EntityId = semester.Id,
                Action = "CREATE",
                UserId = user.Id ?? 0,
                CreateAt = DateTime.Now
            });

            List<SemesterWeekDTO> weeks = new();

            if (startAt.HasValue && endAt.HasValue)
            {
                var startDateOnly = DateOnly.FromDateTime(startAt.Value);
                var endDateOnly = DateOnly.FromDateTime(endAt.Value);

                weeks = SemesterHelper.GetWeeks(startDateOnly, endDateOnly, semester.Id);

                int learnWeekCount = 0;
                foreach (var w in weeks)
                {
                    if (w.IsVacation != null)
                    {
                        learnWeekCount++;
                        w.WeekLearn = learnWeekCount;
                    }
                }

                var semesterWeeks = weeks.Select(w => new SemesterWeek
                {
                    SemesterId = semester.Id,
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    StartAtLunar = SafeConvertToLunar(w.StartAt ?? DateTime.Now),
                    EndAtLunar = SafeConvertToLunar(w.EndAt ?? DateTime.Now),
                    IsVacation = w.IsVacation,
                    WeekLearn = w.WeekLearn
                }).ToList();

                await _context.SemesterWeeks.AddRangeAsync(semesterWeeks);
                await _context.SaveChangesAsync();

                var activeMilestones = await _context.Milestones
                    .Include(m => m.MilestoneItems)
                    .Where(m => m.IsActive ?? false)
                    .ToListAsync();

                var deliverables = new List<Deliverable>();
                var deliveryItems = new List<DeliveryItem>();

                foreach (var milestone in activeMilestones)
                {
                    var deliverable = new Deliverable
                    {
                        MilestoneId = milestone.Id,
                        SemesterId = semester.Id,
                        Name = milestone.Name,
                        Description = milestone.Description,
                        Deadline = milestone.Deadline,
                        IsActive = true,
                        MajorId = milestone.MajorId
                    };

                    deliverables.Add(deliverable);

                    foreach (var item in milestone.MilestoneItems)
                    {
                        deliveryItems.Add(new DeliveryItem
                        {
                            Name = item.Name,
                            Description = item.Description,
                            MilestoneItemId = item.Id,
                            Deliverable = deliverable
                        });
                    }
                }

                await _context.Deliverables.AddRangeAsync(deliverables);
                await _context.DeliveryItems.AddRangeAsync(deliveryItems);
                await _context.SaveChangesAsync();
            }

            return new SemesterDTO
            {
                Name = semester.Name,
                Description = semester.Description,
                IsActive = semester.IsActive,
                StartAt = semester.StartAt,
                EndAt = semester.EndAt,
                Weeks = weeks
            };
        }

        public async Task<List<SemesterInfoDto>> GetSemestersBySupervisorAsync(int supervisorUserId)
        {
            return await _semesterRepository.GetSemestersBySupervisorAsync(supervisorUserId);
        }

        public async Task<ApiResponse<SemesterDTO>> SyncSemesterByNameAsync(string semesterName)
        {
            if (string.IsNullOrWhiteSpace(semesterName))
                throw new ArgumentException("Semester name cannot be empty.");

            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                throw new UnauthorizedAccessException("User authentication failed.");

            if (!string.Equals(user.Role, RoleEnum.Staff.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Only Staff members can sync semesters.");

            var mockSemester = MockData.AllSemesters
                .FirstOrDefault(s => string.Equals(s.Name.Trim(), semesterName.Trim(), StringComparison.OrdinalIgnoreCase));

            if (mockSemester == null)
                return ApiResponse<SemesterDTO>.Fail($"Mock data for semester '{semesterName}' not found", 400);

            DateTime? startAt = mockSemester.StartAt;
            DateTime? endAt = mockSemester.EndAt;

            if (startAt == null || endAt == null)
                throw new Exception("Mock semester must have valid start and end dates.");

            var semester = await _context.Semesters
                .Include(s => s.SemesterWeeks)
                .FirstOrDefaultAsync(s => s.Name.Trim().ToLower() == semesterName.Trim().ToLower());

            if (semester == null)
                throw new Exception($"Semester '{semesterName}' not found in database.");

            bool hasTimeChange = semester.StartAt != startAt || semester.EndAt != endAt;
            bool hasDescChange = semester.Description?.Trim() != mockSemester.Description?.Trim();

            if (hasTimeChange || hasDescChange)
            {
                semester.StartAt = startAt;
                semester.EndAt = endAt;
                semester.Description = mockSemester.Description;
                /*semester.IsActive = true;*/

                _context.SemesterWeeks.RemoveRange(semester.SemesterWeeks);

                var startDateOnly = DateOnly.FromDateTime(startAt.Value);
                var endDateOnly = DateOnly.FromDateTime(endAt.Value);
                var weeks = SemesterHelper.GetWeeks(startDateOnly, endDateOnly, semester.Id);

                int learnWeekCount = 0;
                foreach (var w in weeks)
                {
                    if (w.IsVacation != null)
                    {
                        learnWeekCount++;
                        w.WeekLearn = learnWeekCount;
                    }
                }

                var semesterWeeks = weeks.Select(w => new SemesterWeek
                {
                    SemesterId = semester.Id,
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    StartAtLunar = SafeConvertToLunar(w.StartAt ?? DateTime.Now),
                    EndAtLunar = SafeConvertToLunar(w.EndAt ?? DateTime.Now),
                    IsVacation = w.IsVacation,
                    WeekLearn = w.WeekLearn
                }).ToList();

                await _context.SemesterWeeks.AddRangeAsync(semesterWeeks);

                await _context.SaveChangesAsync();

                await _logService.AddLogAsync(new Log
                {
                    Name = "Sync semester by name",
                    EntityName = "Semester",
                    EntityId = semester.Id,
                    Action = "UPDATE",
                    Description = $"Synchronized semester '{semester.Name}' from mock data: {semester.StartAt:yyyy-MM-dd} to {semester.EndAt:yyyy-MM-dd}.",
                    UserId = user.Id ?? 0,
                    CreateAt = DateTime.Now
                });
            }

            // ✅ Trả về DTO
            var dtoWeeks = await _context.SemesterWeeks
                .Where(w => w.SemesterId == semester.Id)
                .Select(w => new SemesterWeekDTO
                {
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    IsVacation = w.IsVacation,
                    WeekLearn = w.WeekLearn
                }).ToListAsync();

            var dto = new SemesterDTO
            {
                Name = semester.Name,
                Description = semester.Description,
                IsActive = semester.IsActive,
                StartAt = semester.StartAt,
                EndAt = semester.EndAt,
                Weeks = dtoWeeks
            };

            return ApiResponse<SemesterDTO>.Success(dto, "Semester synced successfully.");
        }

        private DateTime SafeConvertToLunar(DateTime date)
        {
            try
            {
                return SemesterHelper.ConvertSolarToLunar(date);
            }
            catch
            {
                _logger.LogError($"Lunar conversion failed for date {date:yyyy-MM-dd}");
                return date; 
            }
        }

        public async Task<bool> IsOverlappingAsync(DateOnly start, DateOnly end)
        {
            var startDateTime = start.ToDateTime(TimeOnly.MinValue);
            var endDateTime = end.ToDateTime(TimeOnly.MaxValue);
            return await _context.Semesters.AnyAsync(s =>
      s.StartAt.HasValue && s.EndAt.HasValue &&
      startDateTime <= s.EndAt.Value && endDateTime >= s.StartAt.Value);
        }

        public async Task<List<SemesterDTO>> GetAllSemestersAsync()
        {
            var semesters = await _semesterRepository.getAllSemesters();

            return semesters.Select(s => new SemesterDTO
            {
                Id = s.Id,
                IsActive = s.IsActive,
                Name = s.Name ?? "",
                StartAt = s.StartAt ?? default,
                EndAt = s.EndAt ?? default,
                Description = s.Description ?? "",
                Weeks = s.SemesterWeeks.Select(w => new SemesterWeekDTO
                {
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    StartAtLunar = w.StartAtLunar,
                    EndAtLunar = w.EndAtLunar,
                }).ToList(),
                SemesterBreak = s.SemesterVacations?
                    .Select(w => new SemesterVacationDto
                    {
                        id = w.Id,
                        StartDate = w.StartAt ?? DateTime.MinValue,
                        EndDate = w.EndAt ?? DateTime.MinValue,
                        Description = w.Description
                    }).ToList()

            }).ToList();
        }

        public async Task<SemesterDTO?> GetSemesterByIdAsync(int id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null) return null;

            return new SemesterDTO
            {
                Id = semester.Id,
                IsActive = semester.IsActive,
                Name = semester.Name ?? "",
                StartAt = semester.StartAt ?? default,
                EndAt = semester.EndAt ?? default,
                Description = semester.Description ?? "",
                Weeks = semester.SemesterWeeks?.Select(w => new SemesterWeekDTO
                {
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    StartAtLunar = w.StartAtLunar,
                    EndAtLunar = w.EndAtLunar,
                }).ToList(),
                SemesterBreak = semester.SemesterVacations?
                    .Select(w => new SemesterVacationDto
                    {
                        id = w.Id,
                        StartDate = w.StartAt ?? DateTime.MinValue,
                        EndDate = w.EndAt ?? DateTime.MinValue,  
                        Description = w.Description
                    }).ToList()
            };
        }

        /* public async Task<SemesterDTO> UpdateSemesterAsync(int id, SemesterUpdateRequest semesterData)
         {
             var user = await _authUtils.GetUserInfoFromCookie();
             if (user == null)
                 throw new UnauthorizedAccessException("User authentication failed.");

             if (!string.Equals(user.Role, RoleEnum.Staff.ToString(), StringComparison.OrdinalIgnoreCase))
                 throw new UnauthorizedAccessException("Only Staff members are allowed to access semester details.");
             if (id <= 0)
                 throw new ArgumentException("Semester ID must be greater than 0.");

             if (semesterData == null)
                 throw new ArgumentNullException(nameof(semesterData), "Request data cannot be null.");

             if (string.IsNullOrWhiteSpace(semesterData.Name))
                 throw new ArgumentException("Semester name cannot be empty.");

             if (semesterData.StartAt == null || semesterData.EndAt == null)
                 throw new ArgumentException("Start date and end date cannot be null.");

             if (semesterData.StartAt >= semesterData.EndAt)
                 throw new ArgumentException("Start date must be earlier than end date.");
             // 1️⃣ Lấy học kỳ hiện tại
             var semester = await _context.Semesters
                 .Include(s => s.SemesterWeeks)
                 .FirstOrDefaultAsync(s => s.Id == id);

             if (semester == null)
                 throw new Exception("Không tìm thấy học kỳ.");

             bool timeChanged = semesterData.StartAt != semester.StartAt || semesterData.EndAt != semester.EndAt;
             if (semesterData.IsActive == true)
             {
                 var allSemesters = await _context.Semesters
                     .Where(s => s.Id != id && s.IsActive == true)
                     .ToListAsync();

                 foreach (var s in allSemesters)
                     s.IsActive = false;

                 await _context.SaveChangesAsync();

                 semester.IsActive = true;
             }
             // 2️⃣ Kiểm tra thời gian
             if (timeChanged && semesterData.StartAt.HasValue && semesterData.EndAt.HasValue)
             {
                 if (semesterData.StartAt >= semesterData.EndAt)
                     throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

                 bool overlap = await _context.Semesters
                     .AnyAsync(s =>
                         s.Id != id &&
                         (
                             semesterData.StartAt >= s.StartAt && semesterData.StartAt <= s.EndAt
                             || semesterData.EndAt >= s.StartAt && semesterData.EndAt <= s.EndAt
                             || semesterData.StartAt <= s.StartAt && semesterData.EndAt >= s.EndAt
                         )
                     );

                 if (overlap)
                     throw new Exception("Khoảng thời gian bị trùng với kỳ học khác trong hệ thống.");
             }

             // 3️⃣ Cập nhật thông tin chung
             semester.Name = semesterData.Name ?? semester.Name;
             semester.Description = semesterData.Description ?? semester.Description;
             semester.IsActive = semesterData.IsActive ?? semester.IsActive;
             semester.StartAt = semesterData.StartAt ?? semester.StartAt;
             semester.EndAt = semesterData.EndAt ?? semester.EndAt;

             // 4️⃣ Nếu thời gian thay đổi → cập nhật lại danh sách tuần
             if (timeChanged)
             {
                 _context.SemesterWeeks.RemoveRange(semester.SemesterWeeks);

                 var start = DateOnly.FromDateTime(semester.StartAt ?? DateTime.Now);
                 var end = DateOnly.FromDateTime(semester.EndAt ?? DateTime.Now);
                 var newWeeks = SemesterHelper.GetWeeks(start, end, semester.Id);

                 var semesterWeeks = newWeeks.Select(w => new SemesterWeek
                 {
                     SemesterId = semester.Id,
                     WeekNumber = w.WeekNumber,
                     StartAt = w.StartAt,
                     EndAt = w.EndAt,
                     StartAtLunar = SemesterHelper.ConvertSolarToLunar(w.StartAt ?? DateTime.Now),
                     EndAtLunar = SemesterHelper.ConvertSolarToLunar(w.EndAt ?? DateTime.Now),
                 }).ToList();

                 await _context.SemesterWeeks.AddRangeAsync(semesterWeeks);
             }

             try
             {
                 await _context.SaveChangesAsync();
                 await _logService.AddLogAsync(new Log
                 {
                     Name = "Cập nhật kỳ học",
                     EntityName = "Semester",
                     EntityId = semester.Id,
                     Action = "UPDATE",
                     Description = $"Cập nhật kỳ học {semester.Name} (thời gian: {semester.StartAt:yyyy-MM-dd} - {semester.EndAt:yyyy-MM-dd})",
                     UserId = user.Id ?? 0, 
                     CreateAt = DateTime.Now
                 });
             }
             catch (DbUpdateException ex)
             {
                 throw new Exception("Lỗi khi lưu thay đổi: " + ex.InnerException?.Message);
             }

             var weeks = await _context.SemesterWeeks
                 .Where(w => w.SemesterId == semester.Id)
                 .Select(w => new SemesterWeekDTO
                 {
                     WeekNumber = w.WeekNumber,
                     StartAt = w.StartAt,
                     EndAt = w.EndAt,
                     IsVacation = w.IsVacation
                 }).ToListAsync();

             return new SemesterDTO
             {
                 Name = semester.Name ?? "",
                 StartAt = semester.StartAt ?? default,
                 EndAt = semester.EndAt ?? default,
                 Description = semester.Description ?? "",
                 Weeks = weeks,
                 SemesterBreak = semester.SemesterVacations?
                     .Select(w => new SemesterVacationDto
                     {
                         id = w.Id,
                         StartDate = w.StartAt ?? DateTime.MinValue,
                         EndDate = w.EndAt ?? DateTime.MinValue,
                         Description = w.Description
                     }).ToList()
             };
         }*/
        public async Task<SemesterDTO> UpdateSemesterAsync(int id, SemesterUpdateRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Semester ID must be greater than 0.");
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request data cannot be null.");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Semester name cannot be empty.");

            // ✅ Lấy thông tin người dùng
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                throw new UnauthorizedAccessException("User authentication failed.");
            if (!string.Equals(user.Role, RoleEnum.Staff.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Only Staff members can update semesters.");

            // ✅ Lấy kỳ từ DB
            var semester = await _context.Semesters
                .Include(s => s.SemesterWeeks)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (semester == null)
                throw new Exception("Không tìm thấy học kỳ.");

            // ✅ Map StartAt / EndAt từ MockData nếu có
            var mockSemester = MockData.AllSemesters
                .FirstOrDefault(s => string.Equals(s.Name.Trim(), request.Name.Trim(), StringComparison.OrdinalIgnoreCase));

            DateTime? startAt = mockSemester?.StartAt;
            DateTime? endAt = mockSemester?.EndAt;

            // ✅ Cập nhật thông tin cơ bản
            semester.Name = request.Name.Trim();
            semester.Description = request.Description?.Trim();
            semester.StartAt = startAt;
            semester.EndAt = endAt;
            semester.IsActive = startAt.HasValue && endAt.HasValue;

            // ✅ Nếu có start/end → xử lý disable kỳ đang active khác
            if (startAt.HasValue && endAt.HasValue)
            {
                var activeSemester = await _context.Semesters
                    .FirstOrDefaultAsync(s => s.IsActive == true && s.Id != id);
                if (activeSemester != null)
                {
                    activeSemester.IsActive = false;
                    _context.Semesters.Update(activeSemester);
                }

                // Xóa tuần cũ
                _context.SemesterWeeks.RemoveRange(semester.SemesterWeeks);

                // Tạo lại tuần mới
                var startDateOnly = DateOnly.FromDateTime(startAt.Value);
                var endDateOnly = DateOnly.FromDateTime(endAt.Value);
                var newWeeks = SemesterHelper.GetWeeks(startDateOnly, endDateOnly, semester.Id);

                int learnWeekCount = 0;
                foreach (var w in newWeeks)
                {
                    if (w.IsVacation != null)
                    {
                        learnWeekCount++;
                        w.WeekLearn = learnWeekCount;
                    }
                }

                var semesterWeeks = newWeeks.Select(w => new SemesterWeek
                {
                    SemesterId = semester.Id,
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    StartAtLunar = SemesterHelper.ConvertSolarToLunar(w.StartAt ?? DateTime.Now),
                    EndAtLunar = SemesterHelper.ConvertSolarToLunar(w.EndAt ?? DateTime.Now),
                    IsVacation = w.IsVacation,
                    WeekLearn = w.WeekLearn
                }).ToList();

                await _context.SemesterWeeks.AddRangeAsync(semesterWeeks);
            }

            try
            {
                await _context.SaveChangesAsync();

                // ✅ Log cập nhật
                await _logService.AddLogAsync(new Log
                {
                    Name = "Cập nhật học kỳ",
                    EntityName = "Semester",
                    EntityId = semester.Id,
                    Action = "UPDATE",
                    Description = startAt.HasValue && endAt.HasValue
                        ? $"Cập nhật học kỳ '{semester.Name}' từ {semester.StartAt:yyyy-MM-dd} đến {semester.EndAt:yyyy-MM-dd}."
                        : $"Cập nhật học kỳ '{semester.Name}' không có thời gian bắt đầu/kết thúc.",
                    UserId = user.Id ?? 0,
                    CreateAt = DateTime.Now
                });
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi lưu thay đổi: " + ex.InnerException?.Message);
            }

            // ✅ Lấy lại danh sách tuần mới (nếu có)
            var weeks = await _context.SemesterWeeks
                .Where(w => w.SemesterId == semester.Id)
                .Select(w => new SemesterWeekDTO
                {
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    IsVacation = w.IsVacation,
                    WeekLearn = w.WeekLearn
                }).ToListAsync();

            return new SemesterDTO
            {
                Name = semester.Name,
                Description = semester.Description,
                StartAt = semester.StartAt,
                EndAt = semester.EndAt,
                IsActive = semester.IsActive,
                Weeks = weeks
            };
        }


        public async Task<Semester?> GetSemesterByNow()
        {
            return await _semesterRepository.GetSemesterByNow();
        }

        public async Task<SemesterDeliveriesDTO?> GetMilestonesBySemester(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Semester ID must be greater than 0.");

            var semester = await _semesterRepository.GetMilestonesBySemester(id);
            if (semester == null) return null;

            return new SemesterDeliveriesDTO
            {
                Id = semester.Id,
                Name = semester.Name,
                Deliverables = semester.Deliverables.Select(d => new DeliverableDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Deadline = d.Deadline,
                    Milestone = new MilestoneDTO
                    {
                        Id = d.Milestone.Id,
                        Name = d.Milestone.Name,
                        Description = d.Milestone.Description
                    }
                }).ToList()
            };
        }

        public async Task<SemesterDeliveriesDTO?> GetDeliveriesBySemester(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Semester ID must be greater than 0.");
            var semester = await _semesterRepository.GetDeliveriesBySemester(id);
            if (semester == null) throw new Exception("Id của kì không tồn tại trong hệ thống");
            return new SemesterDeliveriesDTO
            {
                Id = semester.Id,
                Name = semester.Name,
                Deliverables = semester.Deliverables.Select(d => new DeliverableDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Deadline = d.Deadline,
                    Milestone = d.Milestone == null ? null : new MilestoneDTO
                    {
                        Id = d.Milestone.Id,
                        Name = d.Milestone.Name,
                        Description = d.Milestone.Description
                    }
                }).ToList()
            };
        }

        public Task<Semester?> GetGroupsBySemester(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> AddVacationsAsync(List<SemesterVacationRequestDto> vacations)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                throw new UnauthorizedAccessException("User authentication failed.");

            if (!string.Equals(user.Role, RoleEnum.Staff.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Only staff members are allowed to add semester vacations.");

            if (vacations == null || vacations.Count == 0)
                return new ApiResponse<string>(400, "Vacation list cannot be empty.");

            foreach (var v in vacations)
            {
                if (v.SemesterId <= 0)
                    return new ApiResponse<string>(400, "SemesterId must be greater than 0.");

                if (v.StartDate == default || v.EndDate == default)
                    return new ApiResponse<string>(400, "StartDate and EndDate are required and must be valid dates.");

                if (v.StartDate >= v.EndDate)
                    return new ApiResponse<string>(400, $"StartDate must be earlier than EndDate (Vacation: {v.Description ?? "Unnamed"}).");

                if (string.IsNullOrWhiteSpace(v.Description))
                    return new ApiResponse<string>(400, "Vacation description cannot be empty.");

                var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == v.SemesterId);
                if (semester == null)
                    return new ApiResponse<string>(400, $"Semester with ID {v.SemesterId} not found.");

                if (v.StartDate < semester.StartAt || v.EndDate > semester.EndAt)
                    return new ApiResponse<string>(400,
                        $"Vacation '{v.Description}' ({v.StartDate:yyyy-MM-dd} → {v.EndDate:yyyy-MM-dd}) must be within the semester period " +
                        $"({semester.StartAt:yyyy-MM-dd} → {semester.EndAt:yyyy-MM-dd}).");

                bool isOverlapping = await _context.SemesterVacations
                    .AnyAsync(sv => sv.SemesterId == v.SemesterId &&
                                    ((v.StartDate >= sv.StartAt && v.StartDate <= sv.EndAt) ||
                                     (v.EndDate >= sv.StartAt && v.EndDate <= sv.EndAt) ||
                                     (v.StartDate <= sv.StartAt && v.EndDate >= sv.EndAt)));

                if (isOverlapping)
                    return new ApiResponse<string>(400,
                        $"Vacation '{v.Description}' ({v.StartDate:yyyy-MM-dd} → {v.EndDate:yyyy-MM-dd}) overlaps with another existing vacation.");
            }

            var success = await _semesterRepository.AddVacationsAsync(vacations);
            if (!success)
                return new ApiResponse<string>(500, "Failed to add semester vacations due to a server error.");

            var description = string.Join("; ", vacations.Select(v =>
                $"{v.Description} ({v.StartDate:yyyy-MM-dd} → {v.EndDate:yyyy-MM-dd})"));

            await _logService.AddLogAsync(new Log
            {
                Name = "Add semester vacations",
                EntityName = "SemesterVacation",
                Action = "CREATE",
                Description = $"User ID {user.Id} added vacations: {description}",
                UserId = user.Id ?? 0,
                CreateAt = DateTime.Now
            });

            return new ApiResponse<string>(200, "Semester vacations added successfully.");
        }

        public async Task<ApiResponse<string>> UpdateSemesterVacationsAsync(int semesterId, List<SemesterUpdateVacationRequestDto> vacationDtos)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null)
                throw new UnauthorizedAccessException("User authentication failed.");

            if (!string.Equals(user.Role, RoleEnum.Staff.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Only staff members are allowed to add semester vacations.");

            if (semesterId <= 0)
                return new ApiResponse<string>(400, "SemesterId must be greater than 0.");

            if (vacationDtos == null || vacationDtos.Count == 0)
                return new ApiResponse<string>(400, "Vacation list cannot be empty.");

            var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == semesterId);
            if (semester == null)
                return new ApiResponse<string>(400, $"Semester with ID {semesterId} not found.");

            // Delete all old vacations
            var oldVacations = await _context.SemesterVacations
                .Where(v => v.SemesterId == semesterId)
                .ToListAsync();

            _context.SemesterVacations.RemoveRange(oldVacations);

            foreach (var dto in vacationDtos)
            {
                if (dto.StartDate == default || dto.EndDate == default)
                    return new ApiResponse<string>(400, "StartDate and EndDate cannot be empty or invalid.");

                if (dto.StartDate >= dto.EndDate)
                    return new ApiResponse<string>(400, "StartDate must be earlier than EndDate.");

                if (string.IsNullOrWhiteSpace(dto.Description))
                    return new ApiResponse<string>(400, "Vacation description cannot be empty.");

                if (dto.StartDate < semester.StartAt || dto.EndDate > semester.EndAt)
                    return new ApiResponse<string>(400,
                        $"Vacation period must be within the semester duration ({semester.StartAt:yyyy-MM-dd} → {semester.EndAt:yyyy-MM-dd}).");
            }

            var ordered = vacationDtos.OrderBy(v => v.StartDate).ToList();
            for (int i = 0; i < ordered.Count - 1; i++)
            {
                if (ordered[i].EndDate > ordered[i + 1].StartDate)
                    return new ApiResponse<string>(400, "Vacation periods overlap. Please adjust the dates.");
            }

            var newVacations = vacationDtos.Select(dto => new SemesterVacation
            {
                SemesterId = semesterId,
                StartAt = dto.StartDate,
                EndAt = dto.EndDate,
                Description = dto.Description
            }).ToList();

            await _context.SemesterVacations.AddRangeAsync(newVacations);
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(new Log
            {
                Name = "Update semester vacation list",
                EntityName = "SemesterVacation",
                EntityId = semesterId,
                Action = "UPDATE",
                Description = $"User ID {user.Id} updated all vacations for semester {semesterId}.",
                UserId = user.Id ?? 0,
                CreateAt = DateTime.Now
            });

            return new ApiResponse<string>(200, "Semester vacation list updated successfully.");
        }



        public Task<ApiResponse<List<SemesterVacationDto>>> GetBySemesterIdAsync(int semesterId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<List<SemesterVacationDto>>> GetVacationsBySemesterAsync(int semesterId)
        {
            if (semesterId <= 0)
                throw new ArgumentException("Semester ID must be greater than 0.");

            var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == semesterId);
            if (semester == null)
                return new ApiResponse<List<SemesterVacationDto>>(400, $"Không tìm thấy học kỳ ID {semesterId}.");

            var vacations = await _semesterRepository.GetVacationsBySemesterAsync(semesterId);

            var data = vacations.Select(v => new SemesterVacationDto
            {
                id = v.Id,
                StartDate = v.StartAt ?? DateTime.MinValue,
                EndDate = v.EndAt ?? DateTime.MinValue,
                Description = v.Description
            }).ToList();

            return new ApiResponse<List<SemesterVacationDto>>(200, "Lấy danh sách kỳ nghỉ thành công.", data);
        }

    }
}
