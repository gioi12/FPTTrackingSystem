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
        Task<ApiResponse<string>> UpdateIsActiveAsync(int campusId, int slotId, bool isActive);
        Task<ApiResponse<object>> CreateCampusAsync(CreateCampusDto dto);
        Task<ApiResponse<object>> UpdateCampusAsync(int id, UpdateCampusDto dto);
        Task<ApiResponse<object>> DeleteCampusAsync(int id);
        Task<ApiResponse<List<SlotCampusDto>>> CreateSlotsAsync(int campusId,List<SlotCreateDto> slots,string? userRole);
    }
}
