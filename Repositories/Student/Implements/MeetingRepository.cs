using Entities.Models;
using Repositories.Student.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTranferObjects.Student.Meeting;

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
        public async Task<List<FreeTimeSlotDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId)
        {
            return await _context.GroupUsers
                .Where(f => f.GroupId == groupId)
                .GroupBy(f => new { f.UserId, f.GroupId, f.DayOfWeek })
                .Select(g => new FreeTimeSlotDto
                {
                    StudentId = g.Key.UserId,
                    GroupId = g.Key.GroupId,
                    DayOfWeek = g.Key.DayOfWeek,
                    TimeSlots = g.Select(x => x.FreeTime).ToList()
                })
                .ToListAsync();
        }
    }
}
