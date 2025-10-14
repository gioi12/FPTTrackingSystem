using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using DataTranferObjects.Staff.Semester;
using Entities.Models;
using FPTTrackingSystem.Hepler;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff;

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
                    x.Code
                }).Cast<object>().ToList()
            };
            return ApiResponse<SemesterActiveRes>.Success(se);
        }

        /*public async Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest request)
        {
            if (!DateTime.TryParse(request.StartAt, out var startAtDateTime) ||
                !DateTime.TryParse(request.EndAt, out var endAtDateTime))
            {
                throw new Exception("Ngày không hợp lệ. Định dạng phải là yyyy-MM-dd.");
            }

            var startAt = DateOnly.FromDateTime(startAtDateTime);
            var endAt = DateOnly.FromDateTime(endAtDateTime);

            if (startAt >= endAt)
            {
                throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");
            }

            // Chỉ có 1 kỳ active
            var activeSemester = await _context.Semesters.FirstOrDefaultAsync(s => s.IsActive ?? false);
            if (activeSemester != null)
            {
                activeSemester.IsActive = false;
            }

            var semester = new Semester
            {
                Name = request.Name,
                StartAt = startAtDateTime,
                EndAt = endAtDateTime,
                Description = request.Description,
                IsActive = true
            };

            try
            {
                _context.Semesters.Add(semester);
                await _context.SaveChangesAsync();

                _logService.AddLog(new Log
                {
                    Name = "Tạo kỳ học mới",
                    EntityName = "Semester",
                    EntityId = semester.Id,
                    Action = "CREATE",
                    Description = $"Tạo kỳ học {semester.Name} từ {semester.StartAt:yyyy-MM-dd} đến {semester.EndAt:yyyy-MM-dd}",
                    UserId = 1,
                    CreateAt = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu dữ liệu: {ex.InnerException?.Message ?? ex.Message}");
            }

            var weeks = SemesterHelper.GetWeeks(startAt, endAt, semester.Id);

            int learnWeekCount = 0;
            foreach (var w in weeks)
            {
                if (w.IsVacation == false)
                {
                    learnWeekCount++;
                    w.WeekLearn = learnWeekCount;
                }
                else
                {
                    w.WeekLearn = null;
                }
            }
            var semesterWeeks = weeks.Select(w => new SemesterWeek
            {
                SemesterId = semester.Id,
                WeekNumber = w.WeekNumber,
                StartAt = w.StartAt,
                EndAt = w.EndAt,
                IsVacation = w.IsVacation,
                WeekLearn = w.WeekLearn
            }).ToList();

            _context.SemesterWeeks.AddRange(semesterWeeks);
            await _context.SaveChangesAsync();

            return new SemesterDTO
            {
                Name = semester.Name ?? string.Empty,
                StartAt = semester.StartAt ?? default,
                EndAt = semester.EndAt ?? default,
                Weeks = weeks,
                Description = semester.Description,
                IsActive = false,
            };
        }*/

        public async Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest request)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            // 1️⃣ Validate ngày
            if (!DateTime.TryParse(request.StartAt, out var startAtDateTime) ||
                !DateTime.TryParse(request.EndAt, out var endAtDateTime))
                throw new Exception("Ngày không hợp lệ. Định dạng phải là yyyy-MM-dd.");

            var startAt = DateOnly.FromDateTime(startAtDateTime);
            var endAt = DateOnly.FromDateTime(endAtDateTime);
            if (startAt >= endAt)
                throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

            // 2️⃣ Vô hiệu hóa kỳ đang active
            var activeSemester = await _context.Semesters.FirstOrDefaultAsync(s => s.IsActive == true);
            if (activeSemester != null)
            {
                activeSemester.IsActive = false;
                _context.Semesters.Update(activeSemester);
            }

            // 3️⃣ Tạo kỳ mới
            var semester = new Semester
            {
                Name = request.Name,
                StartAt = startAtDateTime,
                EndAt = endAtDateTime,
                Description = request.Description,
                IsActive = true
            };

            try
            {
                await _context.Semesters.AddAsync(semester);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Log lỗi chi tiết của EF Core
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;

                _logger.LogError(dbEx, "Lỗi khi lưu Semester: {Message}", innerMessage);

                throw new Exception($"Lỗi khi lưu dữ liệu Semester: {innerMessage}");
            }
            catch (Exception ex)
            {
                // Log các lỗi khác
                _logger.LogError(ex, "Lỗi không xác định khi lưu Semester");
                throw new Exception($"Lỗi không xác định khi lưu Semester: {ex.Message}");
            }


            // 4️⃣ Log tạo kỳ mới
            _logService.AddLog(new Log
            {
                Name = "Tạo kỳ học mới",
                EntityName = "Semester",
                EntityId = semester.Id,
                Action = "CREATE",
                Description = $"Tạo kỳ học {semester.Name} từ {semester.StartAt:yyyy-MM-dd} đến {semester.EndAt:yyyy-MM-dd}",
                UserId = user.Id ?? 0, 
                CreateAt = DateTime.Now
            });

            // 5️⃣ Sinh tuần học
            var weeks = SemesterHelper.GetWeeks(startAt, endAt, semester.Id);
            int learnWeekCount = 0;
            foreach (var w in weeks)
            {
                if(w.IsVacation != null)
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
                IsVacation = w.IsVacation,
                WeekLearn = w.WeekLearn
            }).ToList();

            await _context.SemesterWeeks.AddRangeAsync(semesterWeeks);
            await _context.SaveChangesAsync();

            // 6️⃣ Lấy toàn bộ Milestone đang active
            var activeMilestones = await _context.Milestones
                .Include(m => m.MilestoneItems)
                .Where(m => m.IsActive == true)
                .ToListAsync();

            // 7️⃣ Tạo Deliverable & DeliveryItem tương ứng
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
                    Deadline = milestone.Deadline
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

            // 8️⃣ Log clone milestone active
            _logService.AddLog(new Log
            {
                Name = "Khởi tạo Deliverable từ Milestone active",
                EntityName = "Deliverable",
                Action = "CREATE",
                Description = $"Tự động sinh {deliverables.Count} Deliverable và {deliveryItems.Count} DeliveryItem từ Milestone active cho kỳ {semester.Name}",
                UserId = user.Id ?? 0,
                CreateAt = DateTime.Now
            });

            // ✅ Return DTO
            return new SemesterDTO
            {
                Name = semester.Name ?? string.Empty,
                StartAt = semester.StartAt ?? default,
                EndAt = semester.EndAt ?? default,
                Description = semester.Description,
                Weeks = weeks,
                IsActive = semester.IsActive ?? false
            };
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
                Name = s.Name ?? "",
                StartAt = s.StartAt ?? default,
                EndAt = s.EndAt ?? default,
                Description = s.Description ?? "",
                IsActive = s.IsActive,
                Weeks = s.SemesterWeeks.Select(w => new SemesterWeekDTO
                {
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    IsVacation = w.IsVacation
                }).ToList(),
                SemesterBreak = s.SemesterWeeks.Where(w => w.IsVacation == true).Select(w => new SemesterWeekDTO
                {
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    IsVacation = w.IsVacation
                }).ToList(),

            }).ToList();
        }

        public async Task<SemesterDTO?> GetSemesterByIdAsync(int id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null) return null;

            return new SemesterDTO
            {
                Id = semester.Id,
                Name = semester.Name ?? "",
                StartAt = semester.StartAt ?? default,
                EndAt = semester.EndAt ?? default,
                Description = semester.Description ?? "",
                IsActive = semester.IsActive,
                Weeks = semester.SemesterWeeks?.Select(w => new SemesterWeekDTO
                {
                    WeekNumber = w.WeekNumber,
                    StartAt = w.StartAt,
                    EndAt = w.EndAt,
                    IsVacation = w.IsVacation
                }).ToList(),
                SemesterBreak = semester.SemesterWeeks?
                    .Where(w => w.IsVacation == true)
                    .Select(w => new SemesterWeekDTO
                    {
                        WeekNumber = w.WeekNumber,
                        StartAt = w.StartAt,
                        EndAt = w.EndAt,
                        IsVacation = w.IsVacation
                    }).ToList()
            };
        }


        public async Task<SemesterDTO> UpdateSemesterAsync(int id, SemesterUpdateRequest request)
        {
            // 1️⃣ Lấy học kỳ hiện tại
            var semester = await _context.Semesters
                .Include(s => s.SemesterWeeks)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (semester == null)
                throw new Exception("Không tìm thấy học kỳ.");

            bool timeChanged = request.StartAt != semester.StartAt || request.EndAt != semester.EndAt;
            if (request.IsActive == true)
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
            if (timeChanged && request.StartAt.HasValue && request.EndAt.HasValue)
            {
                if (request.StartAt >= request.EndAt)
                    throw new Exception("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

                bool overlap = await _context.Semesters
                    .AnyAsync(s =>
                        s.Id != id &&
                        (
                            request.StartAt >= s.StartAt && request.StartAt <= s.EndAt
                            || request.EndAt >= s.StartAt && request.EndAt <= s.EndAt
                            || request.StartAt <= s.StartAt && request.EndAt >= s.EndAt
                        )
                    );

                if (overlap)
                    throw new Exception("Khoảng thời gian bị trùng với kỳ học khác trong hệ thống.");
            }

            // 3️⃣ Cập nhật thông tin chung
            semester.Name = request.Name ?? semester.Name;
            semester.Description = request.Description ?? semester.Description;
            semester.IsActive = request.IsActive ?? semester.IsActive;
            semester.StartAt = request.StartAt ?? semester.StartAt;
            semester.EndAt = request.EndAt ?? semester.EndAt;

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
                    IsVacation = w.IsVacation
                }).ToList();

                await _context.SemesterWeeks.AddRangeAsync(semesterWeeks);
            }

            try
            {
                await _context.SaveChangesAsync();
                _logService.AddLog(new Log
                {
                    Name = "Cập nhật kỳ học",
                    EntityName = "Semester",
                    EntityId = semester.Id,
                    Action = "UPDATE",
                    Description = $"Cập nhật kỳ học {semester.Name} (thời gian: {semester.StartAt:yyyy-MM-dd} - {semester.EndAt:yyyy-MM-dd})",
                    UserId = _authUtils.GetUserInfoFromCookie().Id, 
                    CreateAt = DateTime.Now
                });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("❌ SaveChanges error: " + ex.InnerException?.Message);
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
                Id = semester.Id,
                Name = semester.Name ?? "",
                StartAt = semester.StartAt ?? default,
                EndAt = semester.EndAt ?? default,
                Description = semester.Description ?? "",
                IsActive = semester.IsActive,
                Weeks = weeks,
                SemesterBreak = semester.SemesterWeeks?
                    .Where(w => w.IsVacation == true)
                    .Select(w => new SemesterWeekDTO
                    {
                        WeekNumber = w.WeekNumber,
                        StartAt = w.StartAt,
                        EndAt = w.EndAt,
                        IsVacation = w.IsVacation
                    }).ToList()
            };
        }


    }
}
