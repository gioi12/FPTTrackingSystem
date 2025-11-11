using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
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
        public async Task<IEnumerable<Campus>> GetAllCampusesAsync()
        {
            return await _campusRepository.GetAllCampusesAsync();
        }

        public async Task<Campus?> GetByIdWithSlotsAsync(int campusId) =>
            await _campusRepository.GetByIdWithSlotsAsync(campusId);

        public async Task<Slot> AddSlotAsync(int campusId, Slot slot) =>
            await _campusRepository.AddSlotAsync(campusId, slot);

        public async Task<Slot?> UpdateSlotAsync(int campusId, Slot slot) =>
            await _campusRepository.UpdateSlotAsync(campusId, slot);

        public async Task<bool> DeleteSlotAsync(int campusId, int slotId) =>
            await _campusRepository.DeleteSlotAsync(campusId, slotId);
    }
}
