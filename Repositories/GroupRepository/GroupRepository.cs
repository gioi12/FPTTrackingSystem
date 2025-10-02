using DataTranferObjects.Group;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.GroupRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Group
{
    public class GroupRepository : IGroupRepository
    {
        private readonly FpttrackingSystemContext _context;
        public GroupRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<int> CountAsync(IQueryable<Entities.Models.Group> query)
        {
            return await query.CountAsync();
        }

        public async Task<Entities.Models.Group?> GetByIdAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.Status)
                .Include(g => g.GroupUsers)
                    .ThenInclude(gu => gu.User)
                    .ThenInclude(u => u.Account)
                    .ThenInclude(a => a.Role)
                .Include(g => g.Milestones)
                .FirstOrDefaultAsync(g => g.Id == id);
        }


        public IQueryable<Entities.Models.Group> GetGroupsQuery()
        {
            return _context.Groups
               .Include(g => g.Major)
               .Include(g => g.Semester)
               .Include(g => g.Tasks)
               .Include(g => g.Milestones)
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


    }
}
