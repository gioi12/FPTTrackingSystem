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
        public async Task<List<StudentFreeTimeDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId)
        {
            var data = await _context.GroupUsers
                .Where(f => f.GroupId == groupId &&
                           (f.Role == "Student" || f.Role == "Leader" || f.Role == "Secretary"))
                .Select(f => new
                {
                    f.UserId,
                    f.GroupId,
                    f.DayOfWeek,
                    f.FreeTime
                })
                .ToListAsync();

            var result = data
                .GroupBy(x => new { x.UserId, x.GroupId })
                .Select(g => new StudentFreeTimeDto
                {
                    StudentId = g.Key.UserId,
                    GroupId = g.Key.GroupId,
                    FreeTimeSlots = g
                        .Where(x => !string.IsNullOrEmpty(x.DayOfWeek) && !string.IsNullOrEmpty(x.FreeTime))
                        .SelectMany(x =>
                        {
                            var days = x.DayOfWeek.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(d => d.Trim()).ToList();

                            List<List<string>>? parsed;
                            try
                            {
                                parsed = JsonSerializer.Deserialize<List<List<string>>>(x.FreeTime);
                            }
                            catch
                            {
                                parsed = new List<List<string>>();
                            }

                            return days.Select((day, i) => new FreeTimeSlotByDayDto
                            {
                                DayOfWeek = day,
                                TimeSlots = i < parsed?.Count ? parsed[i] : new List<string>()
                            });
                        })
                        .GroupBy(d => d.DayOfWeek)
                        .Select(d => new FreeTimeSlotByDayDto
                        {
                            DayOfWeek = d.Key,
                            TimeSlots = d.SelectMany(x => x.TimeSlots).Distinct().ToList()
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }

        public async Task<Meeting?> GetMeetingByGroupIdAsync(int groupId)
        {
            return await _context.Groups
                .Where(g => g.Id == groupId)
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

            var group = await _context.Groups
                .Include(g => g.Semester)
                .Include(g => g.Meeting)
                .ThenInclude(m => m.MeetingScheduleDates)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                throw new Exception("Không tìm thấy nhóm.");

            var semester = group.Semester ?? throw new Exception("Nhóm chưa thuộc kỳ học nào.");

            var meeting = group.Meeting;

            bool isNewMeeting = meeting == null;

            if (isNewMeeting)
            {
                meeting = new Meeting
                {
                    DayOfWeek = dto.Day,
                    Time = dto.Time,
                    MeetingLink = dto.MeetingLink,
                    IsActive = true,
                    CreateBy = userId,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                _context.Meetings.Add(meeting);
                group.Meeting = meeting;
            }
            else
            {
                bool dayChanged = !string.Equals(meeting.DayOfWeek, dto.Day, StringComparison.OrdinalIgnoreCase);
                bool timeChanged = meeting.Time != dto.Time;
                bool semesterChanged = meeting.MeetingScheduleDates.Any(d =>
                    d.MeetingDate < semester.StartAt || d.MeetingDate > semester.EndAt);

                meeting.DayOfWeek = dto.Day;
                meeting.Time = dto.Time;
                meeting.MeetingLink = dto.MeetingLink;
                meeting.IsActive = true;
                meeting.UpdateAt = DateTime.UtcNow;

                if (dayChanged || semesterChanged)
                {
                    var oldDates = _context.MeetingScheduleDates.Where(m => m.MeetingId == meeting.Id);
                    _context.MeetingScheduleDates.RemoveRange(oldDates);
                }
            }

            await _context.SaveChangesAsync();

            if (isNewMeeting ||
                !string.Equals(meeting.DayOfWeek, dto.Day, StringComparison.OrdinalIgnoreCase) ||
                meeting.MeetingScheduleDates == null ||
                meeting.MeetingScheduleDates.Count == 0)
            {
                var allDates = calculator.GetAllDatesForDayOfWeek(
                    semester.StartAt!.Value,
                    semester.EndAt!.Value,
                    dto.Day
                );

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
            }

            return meeting;
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

        public async Task<List<MeetingScheduleDate>> GetMeetingScheduleDatesByGroupIdAsync(int groupId)
        {
            return await _context.MeetingScheduleDates
                    .Include(msd => msd.Meeting)
                    .Where(msd => msd.Meeting != null &&
                                  msd.Meeting.Groups.Any(g => g.Id == groupId) &&
                                  msd.IsActive == true)
                    .OrderBy(msd => msd.MeetingDate)
                    .ToListAsync();
        }

        public async Task<MeetingScheduleDate?> GetByIdAsync(int id)
        {
            return await _context.MeetingScheduleDates.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> UpdateAsync(MeetingScheduleDate entity)
        {
            _context.MeetingScheduleDates.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
