using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Campus;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly AuthUtils _authUtils;
        private readonly ICampusService _campusService;
        private readonly FpttrackingSystemContext _context;
        public SlotController(AuthUtils authUtils, ICampusService campusService, FpttrackingSystemContext context)
        {
            _authUtils = authUtils;
            _campusService = campusService;
            _context = context;
        }

        [HttpPost("{campusId}")]
        public async Task<ActionResult<ApiResponse<List<SlotCampusDto>>>> CreateSlotsBatch(
       int campusId,
       [FromBody] List<SlotCreateDto> slots)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Role != RoleEnum.Admin.ToString())
                return Unauthorized(ApiResponse<string>.Unauthorized("Only Admin can create slots"));

            var campus = await _campusService.GetByIdWithSlotsAsync(campusId);
            if (campus == null)
                return Ok(ApiResponse<List<SlotCampusDto>>.Success(new List<SlotCampusDto>(), "Campus not found"));

            var createdSlots = new List<SlotCampusDto>();

            foreach (var s in slots)
            {
                if (!TimeOnly.TryParse(s.StartAt, out var start))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid StartAt format: {s.StartAt}"));

                if (!TimeOnly.TryParse(s.EndAt, out var end))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid EndAt format: {s.EndAt}"));

                if (start >= end)
                    return BadRequest(ApiResponse<string>.Fail($"StartAt must be earlier than EndAt for slot '{s.NameSlot}'"));

                bool overlapInBatch = createdSlots.Any(existing =>
                {
                    var existingStart = TimeOnly.Parse(existing.StartAt);
                    var existingEnd = TimeOnly.Parse(existing.EndAt);
                    return start < existingEnd && end > existingStart;
                });

                if (overlapInBatch)
                    return BadRequest(ApiResponse<string>.Fail($"Slot '{s.NameSlot}' time overlaps with existing slot"));

                var slot = new Slot
                {
                    NameSlot = s.NameSlot,
                    StartAt = start,
                    EndAt = end,
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


        [HttpPut("{campusId}")]
        public async Task<ActionResult<ApiResponse<List<SlotCampusDto>>>> UpdateSlots(int campusId,[FromBody] List<SlotCampusDto> slots)
        {
            // ✅ Check permission
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Role != RoleEnum.Admin.ToString())
                return Unauthorized(ApiResponse<string>.Unauthorized("Only Admin can update slots"));

            if (slots == null || !slots.Any())
                return Ok(ApiResponse<List<SlotCampusDto>>.Success(new List<SlotCampusDto>(), "No slots provided."));

            // ✅ Get campus with existing slots
            var campus = await _context.Campuses
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == campusId);

            if (campus == null)
                return Ok(ApiResponse<List<SlotCampusDto>>.Success(new List<SlotCampusDto>(), $"Campus with ID {campusId} not found."));

            // ✅ Clear existing slots
            if (campus.Slots.Any())
            {
                _context.Slots.RemoveRange(campus.Slots);
                await _context.SaveChangesAsync();
            }

            var createdSlots = new List<SlotCampusDto>();

            foreach (var s in slots)
            {
                if (!TimeOnly.TryParse(s.StartAt, out var start))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid StartAt format: {s.StartAt}"));

                if (!TimeOnly.TryParse(s.EndAt, out var end))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid EndAt format: {s.EndAt}"));

                if (start >= end)
                    return BadRequest(ApiResponse<string>.Fail($"StartAt must be earlier than EndAt for slot '{s.NameSlot}'"));

                bool overlapInBatch = createdSlots.Any(existing =>
                {
                    var existingStart = TimeOnly.Parse(existing.StartAt);
                    var existingEnd = TimeOnly.Parse(existing.EndAt);
                    return start < existingEnd && end > existingStart;
                });

                if (overlapInBatch)
                    return BadRequest(ApiResponse<string>.Fail($"Slot '{s.NameSlot}' overlaps with another slot in the same batch."));
                var slot = new Slot
                {
                    NameSlot = s.NameSlot,
                    StartAt = start,
                    EndAt = end,
                    CampusId = campusId
                };

                _context.Slots.Add(slot);
                await _context.SaveChangesAsync();

                createdSlots.Add(new SlotCampusDto
                {
                    Id = slot.Id,
                    NameSlot = slot.NameSlot!,
                    StartAt = slot.StartAt.ToString(),
                    EndAt = slot.EndAt.ToString()
                });
            }

            return Ok(ApiResponse<List<SlotCampusDto>>.Success(createdSlots, "Slots updated successfully"));
        }
    }
}
