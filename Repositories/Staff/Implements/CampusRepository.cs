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
            return await _context.Campuses
                                 .Include(c => c.Slots)
                                 .ToListAsync();
        }

        public async Task<Campus?> GetByIdWithSlotsAsync(int id)
        {
            return await _context.Campuses.Include(c => c.Slots)
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

        public async Task<Slot?> UpdateSlotAsync(int campusId, Slot slot)
        {
            var campus = await GetByIdWithSlotsAsync(campusId);
            if (campus == null) return null;

            var existingSlot = campus.Slots.FirstOrDefault(s => s.Id == slot.Id);
            if (existingSlot == null) return null;

            existingSlot.NameSlot = slot.NameSlot;
            existingSlot.StartAt = slot.StartAt;
            existingSlot.EndAt = slot.EndAt;

            await _context.SaveChangesAsync();
            return existingSlot;
        }

        public async Task<bool> DeleteSlotAsync(int campusId, int slotId)
        {
            var campus = await GetByIdWithSlotsAsync(campusId);
            if (campus == null) return false;

            var slot = campus.Slots.FirstOrDefault(s => s.Id == slotId);
            if (slot == null) return false;

            _context.Slots.Remove(slot);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
