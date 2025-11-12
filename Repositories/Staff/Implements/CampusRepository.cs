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
            return await _context.Campuses
/*                                 .Include(c => c.Slots)*/
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

        public async Task<List<SlotCampusDto>?> UpdateSlotsAsync(int campusId, List<SlotCampusDto> slots)
        {
            var campus = await _context.Campuses
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == campusId);

            if (campus == null)
                return null;

            // ❌ Xóa toàn bộ slot cũ
            _context.Slots.RemoveRange(campus.Slots);

            // ✅ Thêm mới lại danh sách slot
            var newSlots = new List<Slot>();

            foreach (var s in slots)
            {
                if (string.IsNullOrWhiteSpace(s.StartAt) || string.IsNullOrWhiteSpace(s.EndAt))
                    continue;

                var startTime = TimeOnly.Parse(s.StartAt);
                var endTime = TimeOnly.Parse(s.EndAt);

                if (startTime >= endTime)
                    throw new Exception($"Invalid time range for slot '{s.NameSlot}'. StartAt must be earlier than EndAt.");

                newSlots.Add(new Slot
                {
                    NameSlot = s.NameSlot,
                    StartAt = startTime,
                    EndAt = endTime,
                    CampusId = campusId
                });
            }

            await _context.Slots.AddRangeAsync(newSlots);
            await _context.SaveChangesAsync();

            // ✅ Trả về DTO
            return newSlots.Select(s => new SlotCampusDto
            {
                Id = s.Id,
                NameSlot = s.NameSlot,
                StartAt = s.StartAt.ToString(),
                EndAt = s.EndAt.ToString()
            }).ToList();
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
