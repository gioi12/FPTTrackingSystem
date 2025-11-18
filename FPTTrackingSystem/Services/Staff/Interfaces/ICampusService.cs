using DataTranferObjects.Staff.Campus;
using Entities.Models;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface ICampusService
    {
        Task<IEnumerable<CampusAllDto>> GetAllCampusesAsync();
        Task<CampusDto?> GetByIdWithSlotsAsync(int campusId);
        Task<Slot> AddSlotAsync(int campusId, Slot slot);
        Task<ApiResponse<string>> UpdateIsActiveAsync(int slotId, bool isActive);
    }
}
