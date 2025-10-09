using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Staff
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
          return await  _context.Semesters.FirstOrDefaultAsync(x => x.IsActive == true);
        }

        public async Task<List<Semester>> getAllSemesters()
        {
            return await _context.Semesters
                 .Include(s => s.SemesterWeeks)
                 .OrderByDescending(x => x.StartAt)
                 .ToListAsync();
        }

        public async Task<Semester?> GetSemesterByIdAsync(int id)
        {
            return await _context.Semesters
                .Include(s => s.SemesterWeeks)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

    }
}
