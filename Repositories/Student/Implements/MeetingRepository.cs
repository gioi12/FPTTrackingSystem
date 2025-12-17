using Entities.Models;
using Repositories.Student.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTranferObjects.Student.Meeting;
using System.Text.Json;
using Repositories.Helper;
using DataTranferObjects.Enum;

namespace Repositories.Student.Implements
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly FpttrackingSystemContext _context;

        public MeetingRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }

        public async Task<GroupUser?> GetFreeTimeSlotAsync(int studentId, int groupId)
        {
            return await _context.GroupUsers
                .FirstOrDefaultAsync(g => g.UserId == studentId && g.GroupId == groupId);
        }

        public async Task<bool> CheckStudentInGroupAsync(int studentId, int groupId)
        {
            return await _context.GroupUsers
                .AnyAsync(gu => gu.UserId == studentId && gu.GroupId == groupId && gu.IsActive);
        }

        public async Task<GroupUser> CreateFreeTimeSlotAsync(GroupUser entity)
        {
            await _context.GroupUsers.AddAsync(entity);
            return entity;
        }

        public async Task<GroupUser> UpdateFreeTimeSlotAsync(GroupUser entity)
        {
            _context.GroupUsers.Update(entity);
            return entity;
        }

        public async System.Threading.Tasks.Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        /*public async Task<List<StudentFreeTimeDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId)
        {
            var data = await _context.GroupUsers
                .Where(f => f.GroupId == groupId &&
                           (f.Role == "Student" || f.Role == "Leader" || f.Role == "Secretary" || f.Role == "Member"))
                .Select(f => new
                {
                    f.UserId,
                    f.GroupId,
                    f.DayOfWeek,
                })
                .ToListAsync();

            var result = data
                .GroupBy(x => new { x.UserId, x.GroupId })
                .Select(g => new StudentFreeTimeDto
                {
                    StudentId = g.Key.UserId,
                    GroupId = g.Key.GroupId,
                    FreeTimeSlots = g
                        .Where(x => !string.IsNullOrEmpty(x.DayOfWeek))
                        .GroupBy(d => d.DayOfWeek)
                        .Select(d => new FreeTimeSlotByDayDto
                        {
                            DayOfWeek = d.Key,
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }*/

        public async Task<Meeting?> GetMeetingByGroupIdAsync(int groupId)
        {
            return await _context.Groups
                .Where(g => g.Id == groupId)
                .Include(g => g.Meeting)
                    .ThenInclude(m => m.Slot)
                .Include(g => g.Meeting)
                    .ThenInclude(m => m.CreateByNavigation)
                .Select(g => g.Meeting)
                .FirstOrDefaultAsync();
        }

        /*public async Task<Meeting> FinalizeScheduleAsync(int groupId, FinalMeetingDto dto, int userId)
        {
            var calculator = new CaculateDate();
            var group = await _context.Groups
                .Include(g => g.Semester)
                .Include(g => g.Meeting)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                throw new Exception("Không tìm thấy nhóm.");

            var semester = group.Semester;
            if (semester == null)
                throw new Exception("Nhóm chưa thuộc kỳ học nào.");

            var meeting = group.Meeting;
            if (meeting == null)
            {
                meeting = new Meeting
                {
                    DayOfWeek = dto.Day,
                    Time = dto.Time,
                    MeetingLink = dto.MeetingLink,
                    IsActive = true,
                    CreateBy = userId,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                };
                _context.Meetings.Add(meeting);
                group.Meeting = meeting;
            }
            else
            {
                meeting.DayOfWeek = dto.Day;
                meeting.Time = dto.Time;
                meeting.MeetingLink = dto.MeetingLink;
                meeting.IsActive = true;
                meeting.UpdateAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var oldDates = await _context.MeetingScheduleDates
                .Where(m => m.MeetingId == meeting.Id)
                .ToListAsync();
            if (oldDates.Any())
            {
                _context.MeetingScheduleDates.RemoveRange(oldDates);
            }

            var allDates = calculator.GetAllDatesForDayOfWeek(semester.StartAt!.Value, semester.EndAt!.Value, dto.Day);

            foreach (var date in allDates)
            {
                _context.MeetingScheduleDates.Add(new MeetingScheduleDate
                {
                    MeetingId = meeting.Id,
                    MeetingDate = date,
                    IsActive = true,
                    Description = $"Buổi họp {dto.Day} tuần {calculator.GetWeekNumberInSemester(semester.StartAt.Value, date)}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return meeting;
        }*/
        public async Task<Meeting> FinalizeOrUpdateScheduleAsync(int groupId, FinalMeetingDto dto, int userId)
        {
            var calculator = new CaculateDate();
            var conflict = await _context.Groups
                            .Include(g => g.Meeting)
                            .Include(g => g.GroupUsers)
                            .Where(g =>
                                g.Id != groupId &&                                  
                                g.GroupUsers.Any(gu =>
                                    gu.UserId == userId &&
                                    gu.Role == "Supervisor" &&
                                    gu.IsActive                                     
                                ) &&
                                g.Meeting != null &&
                                g.Meeting.DayOfWeek == dto.Day &&
                                g.Meeting.SlotId == dto.SlotId &&
                                (g.Meeting.IsActive ?? true)
                            )
                            .FirstOrDefaultAsync();

            if (conflict != null)
                throw new Exception($"You already have another group scheduled on {dto.Day} with Slot {dto.SlotId}. A mentor cannot finalize two meetings at the same time.");
            var group = await _context.Groups
                .Include(g => g.Semester)
                .Include(g => g.Meeting)
                    .ThenInclude(m => m.MeetingScheduleDates)
                .Include(g => g.Meeting)
                    .ThenInclude(m => m.Slot)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                throw new Exception("Không tìm thấy nhóm.");

            var semester = group.Semester ?? throw new Exception("Nhóm chưa thuộc kỳ học nào.");
            var meeting = group.Meeting;

            bool isNewMeeting = meeting == null;

                meeting = new Meeting
                {
                    DayOfWeek = dto.Day,
                    SlotId = dto.SlotId,
                    MeetingLink = dto.MeetingLink,
                    IsActive = true,
                    CreateBy = userId,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };

                _context.Meetings.Add(meeting);
                group.Meeting = meeting;
                await _context.SaveChangesAsync();

                var allDates = calculator.GetAllDatesForDayOfWeek(semester.StartAt!.Value,semester.EndAt!.Value,dto.Day);

            var slot = await _context.Slots.FindAsync(dto.SlotId);
            if (slot == null)
                throw new Exception("Không tìm thấy slot.");


            var vacations = await _context.SemesterVacations
                            .Where(v => v.SemesterId == semester.Id)
                            .ToListAsync();

            var scheduleDates = new List<MeetingScheduleDate>();

            foreach (var date in allDates)
            {
                var adjustedDate = GetAdjustedDate(date, vacations);

                var weekNumber = calculator.GetWeekNumberInSemester(
                    semester.StartAt.Value, adjustedDate);

                scheduleDates.Add(new MeetingScheduleDate
                {
                    MeetingId = meeting.Id,
                    MeetingDate = adjustedDate,
                    StartAt = slot.StartAt,
                    EndAt = slot.EndAt,
                    IsActive = true,
                    IsMeeting = false,
                    Description = $"Buổi họp {dto.Day} tuần {weekNumber}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.MeetingScheduleDates.AddRange(scheduleDates);

            await _context.SaveChangesAsync();
            return meeting;
        }

        public bool IsVacationDate(DateTime date, List<SemesterVacation> vacations)
        {
            return vacations.Any(v =>
                v.StartAt.HasValue &&
                v.EndAt.HasValue &&
                date.Date >= v.StartAt.Value.Date &&
                date.Date <= v.EndAt.Value.Date);
        }

        public DateTime GetAdjustedDate(DateTime date, List<SemesterVacation> vacations)
        {
            var adjusted = date;
            while (IsVacationDate(adjusted, vacations))
            {
                adjusted = adjusted.AddDays(1);
            }
            return adjusted;
        }

        public async Task<MeetingMinute> CreateMeetingMinute(MeetingMinute entity)
        {
            await _context.MeetingMinutes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<MeetingMinute?> GetMeetingMinuteByMeetingDate(int meetingDateId)
        {
            return await _context.MeetingMinutes
                .Include(x=>x.CreateByNavigation)
                .FirstOrDefaultAsync(m => m.MeetingScheduleDateId == meetingDateId); 
        }

        public async Task<MeetingMinute?> GetMeetingMinuteById(int id)
        {
            return await _context.MeetingMinutes
                          .Include(x => x.CreateByNavigation)
                          .Include(x => x.Tasks)
                          .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MeetingMinute> UpdateMeetingMinute(MeetingMinute entity)
        {
             _context.MeetingMinutes.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async System.Threading.Tasks.Task DeleteMeetingMinute(MeetingMinute entity)
        {
            if(entity.Tasks != null && entity.Tasks.Count != 0)
            {
                foreach (var item in entity.Tasks)
                {
                    item.IsActive = false;
                    item.MeetingMinuteId = null;
                }
                _context.Tasks.UpdateRange(entity.Tasks);
            }
           
            _context.MeetingMinutes.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Meeting?> GetMeetingByIdAsync(int meetingId)
        {
            return await _context.Meetings
                .Include(m => m.CreateByNavigation)
                .FirstOrDefaultAsync(m => m.Id == meetingId);
        }

        public async Task<MeetingScheduleDate?> GetMeetingDateByIdAsync(int id)
        {
            return await _context.MeetingScheduleDates
                .Include(x=>x.MeetingMinute)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> CheckSecretary(int userId)
        {
            var user = await _context.GroupUsers.FirstOrDefaultAsync(u => u.UserId == userId && u.Role == "Secretary");
            if (user != null) return true;
            return false;
        }

        public async System.Threading.Tasks.Task UpdateMeetingScheduleDate(MeetingScheduleDate m)
        {
            _context.MeetingScheduleDates.Update(m);
            await _context.SaveChangesAsync();
        }


        public async Task<List<MeetingScheduleDate>> GetMeetingScheduleDatesByGroupIdAsync(int groupId)
        {
            return await _context.MeetingScheduleDates
                .Include(x=> x.MeetingMinute)
                .Include(msd => msd.Meeting)
                .Where(msd =>
                    msd.Meeting != null &&
                    msd.Meeting.Groups.Any(g => g.Id == groupId) &&
                    msd.IsActive == true)
                .OrderBy(msd => msd.MeetingDate)
                .ToListAsync();
        }

        public async Task<MeetingScheduleDate?> GetByIdAsync(int id)
        {
            return await _context.MeetingScheduleDates
          .Include(msd => msd.Meeting)
          .FirstOrDefaultAsync(msd => msd.Id == id && msd.IsActive == true);
        }

        public async System.Threading.Tasks.Task UpdateAsync(MeetingScheduleDate entity)
        {
            _context.MeetingScheduleDates.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<MeetingScheduleDate?> GetByIdWithMeetingAndGroupsAsync(int id)
        {
            return await _context.MeetingScheduleDates
                .Include(msd => msd.Meeting)
                    .ThenInclude(m => m.Groups)
                .FirstOrDefaultAsync(msd => msd.Id == id);
        }

        public async Task<List<UserSlot>> GetUserSlotsAsync(int userId, int groupId)
        {
            return await _context.UserSlots
                .Where(us => us.UserId == userId && us.GroupId == groupId)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task DeleteUserSlotsAsync(List<UserSlot> slots)
        {
            _context.UserSlots.RemoveRange(slots);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task AddUserSlotsAsync(List<UserSlot> slots)
        {
            await _context.UserSlots.AddRangeAsync(slots);
            await _context.SaveChangesAsync();
        }

        public async Task<GroupFreeTimeDto> GetFreeTimeSlotsByGroupIdAsync(int groupId)
        {
            var group = await _context.Groups
                .Where(g => g.Id == groupId)
                .Select(g => new GroupFreeTimeDto
                {
                    GroupId = g.Id,
                    Name = g.Name,
                    Students = g.GroupUsers
                        .Where(gu => gu.IsActive)
                        .Select(gu => new StudentFreeTimeDto
                        {
                            StudentId = gu.UserId,
                            GroupId = g.Id,
                            FreeTimeSlots = gu.User.UserSlots
                                .Where(us => us.GroupId == g.Id && us.Slot != null)
                                .GroupBy(us => us.DayOfWeek)
                                .Select(gd => new FreeTimeSlotByDayDto
                                {
                                    DayOfWeek = gd.Key,
                                    TimeSlots = gd.OrderBy(us => us.Slot!.StartAt).
                                    Select(us => new TimeSlotDto
                                    {
                                        Id = us.Slot.Id,
                                        NameSlot = us.Slot.NameSlot,
                                        StartAt = us.Slot!.StartAt ?? TimeOnly.MinValue,
                                        EndAt = us.Slot!.EndAt ?? TimeOnly.MinValue
                                    }).ToList()
                                }).ToList()
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (group == null)
                throw new KeyNotFoundException($"Group with id {groupId} not found.");

            return group;
        }

        public async Task<string> MeetingMinuteData(int groupId)
        {
            var data = await _context.Meetings
                .Include(x => x.MeetingScheduleDates)
                    .ThenInclude(y => y.MeetingMinute)
                .Where(m => m.Groups.Any(g => g.Id == groupId))
                .SelectMany(m => m.MeetingScheduleDates)
                .Where(msd => msd.MeetingMinute != null)
                .Select(msd => new
                {
                    msd.MeetingDate,
                    msd.Description,
                    msd.MeetingMinute.MeetingContent,
                    msd.MeetingMinute.Attendance,
                    msd.MeetingMinute.Issue,
                    msd.MeetingMinute.Other,
                    msd.MeetingMinute.StartAt,
                    msd.MeetingMinute.EndAt
                })
                .ToListAsync();

            if (data == null || data.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var x in data)
            {
                sb.AppendLine($"Meeting Date: {x.MeetingDate:yyyy-MM-dd}");
                sb.AppendLine($"Description: {x.Description}");
                sb.AppendLine($"Time: {x.StartAt:HH:mm} - {x.EndAt:HH:mm}");
                sb.AppendLine("Content:");
                sb.AppendLine(x.MeetingContent);

                sb.AppendLine($"Attendance: {x.Attendance}");
                sb.AppendLine($"Issues: {x.Issue}");
                sb.AppendLine($"Other: {x.Other}");
                sb.AppendLine(new string('-', 40));
            }

            return sb.ToString();
        }

        public Task<List<MeetingMinute>?> GetMeetingMinutesByMeetingId(int meetingId)
        {
            return _context.MeetingScheduleDates
                .Where(x => x.MeetingId == meetingId)
                .Select(x => x.MeetingMinute)
                .ToListAsync();
        }
    }
}
