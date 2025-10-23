using DataTranferObjects.Staff.Group;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Implements
{
    public class GroupRepository : IGroupRepository
    {
        private readonly FpttrackingSystemContext _context;
        public GroupRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<int> CountAsync(IQueryable<Group> query)
        {
            return await query.CountAsync();
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.Status)
                .Include(g => g.GroupUsers)
                    .ThenInclude(gu => gu.User)
                    .ThenInclude(u => u.Account)
                    .ThenInclude(a => a.Role)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public IQueryable<Group> GetGroupsQuery()
        {
            return _context.Groups
               .Include(g => g.Major)
               .Include(g => g.Semester)
               .Include(g => g.Tasks)
               .Include(g => g.GroupUsers)
                   .ThenInclude(gu => gu.User)
                   .ThenInclude(u => u.Account)
                   .ThenInclude(a => a.Role)
               .AsQueryable();
        }

        public async Task<List<DashBoardGroupDto>> GetMajorGroupTotalsAsync()
        {
            return await _context.Majors
            .Select(m => new DashBoardGroupDto
            {
                name = m.Name ?? string.Empty,
                Total = m.MajorCategories.Count()
            })
            .ToListAsync();
        }

        public async Task<List<Group>> GetGroupsActiveSesmester()
        {
            var semester = await _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
            if (semester == null) throw new ValidationException("Not found sesmester currently active");
            return await _context.Groups.Include(x => x.Status)
                  .Where(x => x.SemesterId == semester.Id).ToListAsync();
        }

        public async Task<Group?> GetGroupWithMembersAsync(int groupId)
        {
            return await _context.Groups
                .Include(g => g.GroupUsers)
                    .ThenInclude(gu => gu.User)
                    .ThenInclude(u => u.Account)
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<List<Milestone>> GetMilestonesByMajorAsync(int majorId)
        {
            return await _context.Milestones
                .Where(m => m.MajorId == majorId)
                .OrderBy(m => m.Deadline)
                .ToListAsync();
        }

        public async Task<GroupUser?> GetGroupUserAsync(int groupId, int userId)
        {
            return await _context.GroupUsers
                .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);
        }

        public async Task<List<Group>> GetGroupsByUserIdAsync(int userId)
        {
            return await _context.GroupUsers
        .Include(gu => gu.Group)
            .ThenInclude(g => g.GroupUsers)
                .ThenInclude(gu2 => gu2.User)
        .Where(gu => gu.UserId == userId && gu.Role == "Mentor" && gu.IsActive)
        .Select(gu => gu.Group)
        .ToListAsync();
        }

        public async Task<bool> UpdateRoleInGroupAsync(int groupId, int userId, string newRole)
        {
            var groupUser = await GetGroupUserAsync(groupId, userId);
            if (groupUser == null)
                return false;

            newRole = char.ToUpper(newRole[0]) + newRole.Substring(1).ToLower();

            if (newRole == "Secretary")
            {
                bool alreadyHasSecretary = await _context.GroupUsers
                    .AnyAsync(gu => gu.GroupId == groupId
                                 && gu.Role == "Secretary"
                                 && gu.UserId != userId);

                if (alreadyHasSecretary)
                    throw new InvalidOperationException("Nhóm này đã có một Secretary.");
            }

            groupUser.Role = newRole;
            _context.GroupUsers.Update(groupUser);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
