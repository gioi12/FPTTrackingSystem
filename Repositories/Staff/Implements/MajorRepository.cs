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
    public class MajorRepository : IMajorRepository
    {
        private readonly FpttrackingSystemContext _context;
        public MajorRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<List<Major>> findAll()
        {
            return await _context.Majors.ToListAsync();
        }

        public async Task<List<MajorCategory>> getAllCourse()
        {
            return await _context.MajorCategories.ToListAsync();
        }

        public async Task<List<Major>> getAllMajorAndCode()
        {
            return await _context.Majors
                .Include(m => m.MajorCategories) 
                .ToListAsync();
        }
    }
}
