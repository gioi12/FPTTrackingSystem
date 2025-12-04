using DataTranferObjects.Staff.Campus;
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
    public class CampusRepository : ICampusRepository
    {
        private readonly FpttrackingSystemContext _context;

        public CampusRepository(FpttrackingSystemContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Campus>> GetAllCampusesAsync()
        {
            return await _context.Campuses.Where(c => c.IsActive == true)
/*                                 .Include(c => c.Slots)*/
                                 .ToListAsync();
        }

        public async Task<Campus?> GetByIdWithSlotsAsync(int id)
        {
            return await _context.Campuses.Include(c => c.Slots.Where(s => s.IsActive == true).OrderBy(s => s.StartAt))
                                        .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Slot> AddSlotAsync(int campusId, Slot slot)
        {
            var campus = await GetByIdWithSlotsAsync(campusId);
            if (campus == null) throw new Exception("Campus not found");
            slot.CampusId = campusId;
            _context.Slots.Add(slot);
            await _context.SaveChangesAsync();
            return slot;
        }

        public async Task<Slot?> GetByIdAsync(int id)
        {
            return await _context.Slots.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Slot slot)
        {
            _context.Slots.Update(slot);
            await _context.SaveChangesAsync();
        }

        public async Task<Campus> AddCampusAsync(Campus campus)
        {
            _context.Campuses.Add(campus);
            await _context.SaveChangesAsync();
            return campus;
        }

        public async Task<Campus?> GetCampusByIdAsync(int id)
        {
            return await _context.Campuses.FindAsync(id);
        }

        public async System.Threading.Tasks.Task UpdateCampusAsync(Campus campus)
        {
            _context.Campuses.Update(campus);
            await _context.SaveChangesAsync();
        }

        public async Task<Campus?> GetCampusWithSlotsAsync(int campusId)
        {
            return await _context.Campuses
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == campusId);
        }

        public async System.Threading.Tasks.Task AddSlotsAsync(List<Slot> slots)
        {
            await _context.Slots.AddRangeAsync(slots);
        }

        public async System.Threading.Tasks.Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
