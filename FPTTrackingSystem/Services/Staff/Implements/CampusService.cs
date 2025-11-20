using DataTranferObjects.Staff.Campus;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Implements;
using Repositories.Staff.Interfaces;

namespace FPTTrackingSystem.Services.Staff.Implements
{
    public class CampusService: ICampusService
    {
        private readonly ICampusRepository _campusRepository;
        public CampusService(ICampusRepository campusRepository)
        {
            _campusRepository = campusRepository;
        }
        public async Task<IEnumerable<CampusAllDto>> GetAllCampusesAsync()
        {
            var campuses = await _campusRepository.GetAllCampusesAsync();

            var result = campuses.Select(c => new CampusAllDto
            {
                Id = c.Id,
                Name = c.Name,
/*                Slots = c.Slots.Select(s => new SlotCampusDto
                {
                    Id = s.Id,
                    NameSlot = s.NameSlot!,
                    StartAt = s.StartAt.ToString(),
                    EndAt = s.EndAt.ToString()
                }).ToList()*/
            });

            return result;
        }

        public async Task<CampusDto?> GetByIdWithSlotsAsync(int campusId)
        {
            var campus = await _campusRepository.GetByIdWithSlotsAsync(campusId);
            if (campus == null)
                return null;

            return new CampusDto
            {
                Id = campus.Id,
                Name = campus.Name,
                Slots = campus.Slots.Select(s => new SlotCampusDto
                {
                    Id = s.Id,
                    NameSlot = s.NameSlot!,
                    StartAt = s.StartAt.ToString(),
                    EndAt = s.EndAt.ToString()
                }).ToList()
            };
        }

        public async Task<Slot> AddSlotAsync(int campusId, Slot slot) =>
            await _campusRepository.AddSlotAsync(campusId, slot);
        public async Task<ApiResponse<string>> UpdateIsActiveAsync(int campusId, int slotId, bool isActive)
        {
            var slot = await _campusRepository.GetByIdAsync(slotId);

            if (slot == null)
                return ApiResponse<string>.Fail($"Slot with ID {slotId} not found.");

            if (slot.CampusId != campusId)
                return ApiResponse<string>.Fail("This slot does not belong to the specified campus.");

            slot.IsActive = isActive;

            await _campusRepository.UpdateAsync(slot);

            string statusText = isActive ? "activated" : "deactivated";

            return ApiResponse<string>.Success($"Slot {statusText} successfully.");
        }
    }
}
