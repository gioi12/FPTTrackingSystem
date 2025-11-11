using DataTranferObjects.Staff.Campus;
using Entities.Models;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface ICampusService
    {
        Task<IEnumerable<CampusDto>> GetAllCampusesAsync();
        Task<CampusDto?> GetByIdWithSlotsAsync(int campusId);
        Task<Slot> AddSlotAsync(int campusId, Slot slot);
        Task<List<SlotCampusDto>?> UpdateSlotsAsync(int campusId, List<SlotCampusDto> slots);
        Task<bool> DeleteSlotAsync(int campusId, int slotId);
    }
}
