using DataTranferObjects.Staff.Campus;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly ICampusService _campusService;
        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        [HttpGet()]
        public async Task<IEnumerable<Campus>> GetAllCampusesAsync()
        {
            return await _campusService.GetAllCampusesAsync();
        }

        [HttpPost("{campusId}/slots")]
        public async Task<ActionResult<ApiResponse<List<SlotCampusDto>>>> CreateSlotsBatch(
    int campusId, [FromBody] List<SlotCreateDto> slots)
        {
            var campus = await _campusService.GetByIdWithSlotsAsync(campusId);
            if (campus == null)
                return Ok(ApiResponse<List<SlotCampusDto>>.Success(new List<SlotCampusDto>(), "Campus not found"));

            var createdSlots = new List<SlotCampusDto>();

            foreach (var s in slots)
            {
                var slot = new Slot
                {
                    NameSlot = s.NameSlot,
                    StartAt = TimeOnly.Parse(s.StartAt),
                    EndAt = TimeOnly.Parse(s.EndAt),
                    CampusId = campusId
                };
                var added = await _campusService.AddSlotAsync(campusId, slot);

                createdSlots.Add(new SlotCampusDto
                {
                    Id = added.Id,
                    NameSlot = added.NameSlot!,
                    StartAt = added.StartAt.ToString(),
                    EndAt = added.EndAt.ToString()
                });
            }

            return Ok(ApiResponse<List<SlotCampusDto>>.Success(createdSlots, "Slots created successfully"));
        }

        [HttpPut("{campusId}/slots/{slotId}")]
        public async Task<ActionResult<ApiResponse<Slot>>> UpdateSlot(int campusId, int slotId, Slot slot)
        {
            if (slotId != slot.Id)
                Ok(ApiResponse<Slot>.Success(new Slot(), "Slot ID mismatch"));

            var updated = await _campusService.UpdateSlotAsync(campusId, slot);

            if (updated == null)
                return Ok(ApiResponse<Slot>.Success(new Slot(), "Slot not found"));

            return Ok(ApiResponse<Slot>.Success(updated, "Slot updated successfully"));
        }

        [HttpDelete("{campusId}/slots/{slotId}")]
        public async Task<ActionResult<ApiResponse<Slot>>> DeleteSlot(int campusId, int slotId)
        {
            var deleted = await _campusService.DeleteSlotAsync(campusId, slotId);
            if (!deleted)
                return Ok(ApiResponse<Slot>.Success(new Slot(), "Slot not found"));

            return Ok(ApiResponse<Slot>.Success(new Slot(), "Slot deleted successfully"));
        }
    }
}
