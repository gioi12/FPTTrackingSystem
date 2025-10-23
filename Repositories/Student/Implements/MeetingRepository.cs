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

        public async Task<Meeting> FinalizeScheduleAsync(int groupId, FinalMeetingDto dto, int userId)
        {
            var meeting = await GetMeetingByGroupIdAsync(groupId);

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

                var group = await _context.Groups.FindAsync(groupId);
                if (group != null)
                {
                    group.Meeting = meeting;
                }
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
            return meeting;
        }
    }
}
