using DataTranferObjects.Staff.Campus;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
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
        public async Task<IEnumerable<CampusDto>> GetAllCampusesAsync()
        {
            var campuses = await _campusRepository.GetAllCampusesAsync();

            var result = campuses.Select(c => new CampusDto
            {
                Id = c.Id,
                Name = c.Name,
                Slots = c.Slots.Select(s => new SlotCampusDto
                {
                    Id = s.Id,
                    NameSlot = s.NameSlot!,
                    StartAt = s.StartAt.ToString(),
                    EndAt = s.EndAt.ToString()
                }).ToList()
            });

            return result;
        }

        public async Task<Campus?> GetByIdWithSlotsAsync(int campusId) =>
            await _campusRepository.GetByIdWithSlotsAsync(campusId);

        public async Task<Slot> AddSlotAsync(int campusId, Slot slot) =>
            await _campusRepository.AddSlotAsync(campusId, slot);

        public async Task<List<SlotCampusDto>?> UpdateSlotsAsync(int campusId, List<SlotCampusDto> slots)
        {
            return await _campusRepository.UpdateSlotsAsync(campusId, slots);
        }


        public async Task<bool> DeleteSlotAsync(int campusId, int slotId) =>
            await _campusRepository.DeleteSlotAsync(campusId, slotId);
    }
}
