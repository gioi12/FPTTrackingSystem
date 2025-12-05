using DataTranferObjects.Staff.Group;
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

        public async Task<PagedData<MajorCategory>> GetAllCoursePagedAsync(int page, int pageSize)
        {
            var query = _context.MajorCategories.AsQueryable();

            int total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedData<MajorCategory>
            {
                Items = items,
                Total = total
            };
        }

        public async Task<List<Major>> getAllMajorAndCode()
        {
            return await _context.Majors
                .Include(m => m.MajorCategories) 
                .ToListAsync();
        }

        public async Task<MajorCategory?> GetByIdAsync(int id)
        {
            return await _context.MajorCategories.FindAsync(id);
        }

        public async Task<MajorCategory?> FindByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return await _context.MajorCategories
                .FirstOrDefaultAsync(m => m.Code.ToLower().Trim() == code.ToLower().Trim());
        }

        public async Task<bool> CreateAsync(MajorCategory majorCategory)
        {
            await _context.MajorCategories.AddAsync(majorCategory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MajorCategory majorCategory)
        {
            var existing = await _context.MajorCategories.FindAsync(majorCategory.Id);
            if (existing == null) return false;

            existing.Name = majorCategory.Name;
            existing.Code = majorCategory.Code;
            existing.IsActive = majorCategory.IsActive;
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return true;
        }
    }
}
