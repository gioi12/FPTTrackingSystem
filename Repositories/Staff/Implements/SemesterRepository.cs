using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff.Implements
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly FpttrackingSystemContext _context;
        public SemesterRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<Semester?> findActive()
        {
            return await _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
        }

        public async Task<List<Semester>> getAllSemesters()
        {
            return await _context.Semesters
                 .Include(s => s.SemesterWeeks)
                 .OrderByDescending(x => x.StartAt)
                 .ToListAsync();
        }

        public async Task<Semester?> GetDeliveriesBySemester(int id)
        {
            return await _context.Semesters
                .Include(s => s.Deliverables)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<Semester?> GetGroupsBySemester(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Semester?> GetMilestonesBySemester(int id)
        {
            return await _context.Semesters
                .Include(s => s.Deliverables)
                    .ThenInclude(d => d.Milestone)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Semester?> GetSemesterByIdAsync(int id)
        {
            return await _context.Semesters
                .Include(s => s.SemesterWeeks)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Semester?> GetSemesterByNow()
        {
            return await _context.Semesters.FirstOrDefaultAsync(s => s.IsActive == true);
        }
    }
}
