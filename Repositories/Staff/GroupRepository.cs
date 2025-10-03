using DataTranferObjects.Group;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff
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
                .Include(g => g.Deliverables)
                    .ThenInclude(g => g.Milestone)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public IQueryable<Group> GetGroupsQuery()
        {
            return _context.Groups
               .Include(g => g.Major)
               .Include(g => g.Semester)
               .Include(g => g.Tasks)
               .Include(g => g.Deliverables)
                    .ThenInclude(g => g.Milestone)
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
                Total = m.Groups.Count()
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
    }
}
