using Entities.Models;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface ICampusService
    {
        Task<IEnumerable<Campus>> GetAllCampusesAsync();
        Task<Campus?> GetByIdWithSlotsAsync(int campusId);
        Task<Slot> AddSlotAsync(int campusId, Slot slot);
        Task<Slot?> UpdateSlotAsync(int campusId, Slot slot);
        Task<bool> DeleteSlotAsync(int campusId, int slotId);
    }
}
