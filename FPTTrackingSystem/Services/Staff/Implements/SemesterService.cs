using Azure.Core;
using DataTranferObjects.Staff.Group;
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
using Repositories.Staff.Interfaces;

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
           await _logService.AddLogAsync(new Log
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
                StartAtLunar = SemesterHelper.ConvertSolarToLunar(w.StartAt ?? DateTime.Now),
                EndAtLunar = SemesterHelper.ConvertSolarToLunar(w.EndAt ?? DateTime.Now),
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

            if(activeMilestones == null)
            {
                throw new Exception("Milestone không tồn tại vui lòng tạo milestone.");
            }

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
            await _logService.AddLogAsync(new Log
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

        public async Task<SemesterDTO> UpdateSemesterAsync(int id, SemesterUpdateRequest semesterData)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
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
        }

        public async Task<Semester?> GetSemesterByNow()
        {
            return await _semesterRepository.GetSemesterByNow();
        }

        public async Task<SemesterDeliveriesDTO?> GetMilestonesBySemester(int id)
        {
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
            if (vacations == null || vacations.Count == 0)
                return new ApiResponse<string>(400, "Danh sách nghỉ không được để trống.");

            var user = await _authUtils.GetUserInfoFromCookie();

            foreach (var v in vacations)
            {
                if (v.StartDate >= v.EndDate)
                    return new ApiResponse<string>(400, $"Ngày bắt đầu phải nhỏ hơn ngày kết thúc ({v.Description}).");

                var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == v.SemesterId);
                if (semester == null)
                    return new ApiResponse<string>(400, $"Không tìm thấy học kỳ ID {v.SemesterId}.");

                if (v.StartDate < semester.StartAt || v.EndDate > semester.EndAt)
                    return new ApiResponse<string>(400, $"Thời gian nghỉ '{v.Description}' phải nằm trong khoảng {semester.StartAt:yyyy-MM-dd} → {semester.EndAt:yyyy-MM-dd}.");

                bool isOverlapping = await _context.SemesterVacations
                    .AnyAsync(sv => sv.SemesterId == v.SemesterId &&
                                    ((v.StartDate >= sv.StartAt && v.StartDate <= sv.EndAt) ||
                                     (v.EndDate >= sv.StartAt && v.EndDate <= sv.EndAt) ||
                                     (v.StartDate <= sv.StartAt && v.EndDate >= sv.EndAt)));
                if (isOverlapping)
                    return new ApiResponse<string>(400, $"Khoảng thời gian '{v.Description}' ({v.StartDate:yyyy-MM-dd} → {v.EndDate:yyyy-MM-dd}) bị trùng với kỳ nghỉ khác.");
            }

            var success = await _semesterRepository.AddVacationsAsync(vacations);
            if (!success)
                return new ApiResponse<string>(500, "Thêm thời gian nghỉ thất bại.");

            var description = string.Join("; ", vacations.Select(v =>
                $"{v.Description} ({v.StartDate:yyyy-MM-dd} → {v.EndDate:yyyy-MM-dd})"));

            await _logService.AddLogAsync(new Log
            {
                Name = "Thêm thời gian nghỉ học kỳ",
                EntityName = "SemesterVacation",
                Action = "CREATE",
                Description = $"Người dùng ID {user.Id} đã thêm các kỳ nghỉ: {description}",
                UserId = user.Id ?? 0,
                CreateAt = DateTime.Now
            });

            return new ApiResponse<string>(200, "Thêm thời gian nghỉ thành công.");
        }


        public async Task<ApiResponse<string>> UpdateSemesterVacationsAsync(int semesterId, List<SemesterUpdateVacationRequestDto> vacationDtos)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == semesterId);
            if (semester == null)
                return new ApiResponse<string>(400, $"Không tìm thấy học kỳ ID {semesterId}.");

            // Xóa toàn bộ kỳ nghỉ cũ
            var oldVacations = await _context.SemesterVacations
                .Where(v => v.SemesterId == semesterId)
                .ToListAsync();

            _context.SemesterVacations.RemoveRange(oldVacations);

            // Validate và thêm danh sách mới
            foreach (var dto in vacationDtos)
            {
                if (dto.StartDate >= dto.EndDate)
                    return new ApiResponse<string>(400, "Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

                if (dto.StartDate < semester.StartAt || dto.EndDate > semester.EndAt)
                    return new ApiResponse<string>(400, $"Thời gian nghỉ phải nằm trong khoảng {semester.StartAt:yyyy-MM-dd} → {semester.EndAt:yyyy-MM-dd}.");
            }

            // Kiểm tra chồng lấn trong list mới
            var ordered = vacationDtos.OrderBy(v => v.StartDate).ToList();
            for (int i = 0; i < ordered.Count - 1; i++)
            {
                if (ordered[i].EndDate > ordered[i + 1].StartDate)
                    return new ApiResponse<string>(400, "Các kỳ nghỉ mới bị chồng lấn thời gian.");
            }

            // Thêm mới danh sách
            var newVacations = vacationDtos.Select(dto => new SemesterVacation
            {
                SemesterId = semesterId,
                StartAt = dto.StartDate,
                EndAt = dto.EndDate,
                Description = dto.Description
            }).ToList();

            await _context.SemesterVacations.AddRangeAsync(newVacations);
            await _context.SaveChangesAsync();

            // Log lại
            await _logService.AddLogAsync(new Log
            {
                Name = "Cập nhật danh sách kỳ nghỉ học kỳ",
                EntityName = "SemesterVacation",
                EntityId = semesterId,
                Action = "UPDATE",
                Description = $"Người dùng ID {user.Id} đã cập nhật toàn bộ kỳ nghỉ cho học kỳ {semesterId}.",
                UserId = user.Id ?? 0,
                CreateAt = DateTime.Now
            });

            return new ApiResponse<string>(200, "Cập nhật danh sách kỳ nghỉ thành công.");
        }


        public Task<ApiResponse<List<SemesterVacationDto>>> GetBySemesterIdAsync(int semesterId)
        {
            throw new NotImplementedException();
        }
    }
}
