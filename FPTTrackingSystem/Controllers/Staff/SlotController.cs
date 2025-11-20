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
    [Route("api/")]
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

/*        [HttpPost("v1/slot/{campusId}")]
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
        }*/

        [HttpGet("v1/slot/ById/{campusId}")]
        public async Task<IActionResult> GetCampusByIdAsync(int campusId)
        {
            var campus = await _campusService.GetByIdWithSlotsAsync(campusId);

            if (campus == null)
                return Ok(ApiResponse<CampusDto>.Success(null, $"Campus with ID {campusId} not found."));

            return Ok(ApiResponse<CampusDto>.Success(campus, "Get campus successfully"));
        }


        /*        [HttpPut("v1/slot/{campusId}")]
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
                }*/

        /*[HttpPost("v1/slot/{campusId}")]
        public async Task<ActionResult<ApiResponse<List<SlotCampusDto>>>> CreateSlots(int campusId,[FromBody] List<SlotCreateDto> slots)
        {
            // ✅ Check permission
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Role != RoleEnum.Admin.ToString())
                return Unauthorized(ApiResponse<string>.Unauthorized("Only Admin can create slots"));

            if (slots == null || !slots.Any())
                return BadRequest(ApiResponse<string>.Fail("No slots provided."));

            // ✅ Get campus
            var campus = await _context.Campuses
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == campusId);

            if (campus == null)
                return BadRequest(ApiResponse<string>.Fail($"Campus with ID {campusId} not found."));

            // ✅ Validate slot data
            var validatedSlots = new List<(string Name, TimeOnly Start, TimeOnly End)>();

            foreach (var s in slots)
            {
                if (!TimeOnly.TryParse(s.StartAt, out var start))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid StartAt format: {s.StartAt}"));

                if (!TimeOnly.TryParse(s.EndAt, out var end))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid EndAt format: {s.EndAt}"));

                if (start >= end)
                    return BadRequest(ApiResponse<string>.Fail($"StartAt must be earlier than EndAt for slot '{s.NameSlot}'"));

                // Check overlap with existing campus slots
                bool overlapExisting = campus.Slots.Where(existing => existing.IsActive == true).Any(existing =>
                    start < existing.EndAt && end > existing.StartAt);

                if (overlapExisting)
                    return BadRequest(ApiResponse<string>.Fail($"Slot '{s.NameSlot}' overlaps with existing slot in campus."));

                // Check overlap within batch
                bool overlapInBatch = validatedSlots.Any(existing =>
                    start < existing.End && end > existing.Start);

                if (overlapInBatch)
                    return BadRequest(ApiResponse<string>.Fail($"Slot '{s.NameSlot}' overlaps with another slot in the batch."));

                validatedSlots.Add((s.NameSlot, start, end));
            }

            // ✅ Create new slots
            var createdSlots = new List<SlotCampusDto>();
            foreach (var (name, start, end) in validatedSlots)
            {
                var slot = new Slot
                {
                    NameSlot = name,
                    StartAt = start,
                    EndAt = end,
                    IsActive = true,
                    CampusId = campusId
                };

                _context.Slots.Add(slot);
                createdSlots.Add(new SlotCampusDto
                {
                    NameSlot = slot.NameSlot!,
                    StartAt = slot.StartAt.ToString(),
                    EndAt = slot.EndAt.ToString(),
                });
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<List<SlotCampusDto>>.Success(
                createdSlots,
                $"Created {createdSlots.Count} new slot(s) successfully."
            ));
        }*/
        [HttpPost("v1/slot/{campusId}")]
        public async Task<ActionResult<ApiResponse<List<SlotCampusDto>>>> CreateSlots(
    int campusId,
    [FromBody] List<SlotCreateDto> slots)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Role != RoleEnum.Admin.ToString())
                return Unauthorized(ApiResponse<string>.Unauthorized("Only Admin can create slots"));

            if (slots == null || !slots.Any())
                return BadRequest(ApiResponse<string>.Fail("No slots provided."));

            var campus = await _context.Campuses
                .Include(c => c.Slots)
                .FirstOrDefaultAsync(c => c.Id == campusId);

            if (campus == null)
                return BadRequest(ApiResponse<string>.Fail($"Campus with ID {campusId} not found."));

            var validatedSlots = new List<(string Name, TimeOnly Start, TimeOnly End)>();

            foreach (var s in slots)
            {
                if (!TimeOnly.TryParse(s.StartAt, out var start))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid StartAt format: {s.StartAt}"));

                if (!TimeOnly.TryParse(s.EndAt, out var end))
                    return BadRequest(ApiResponse<string>.Fail($"Invalid EndAt format: {s.EndAt}"));

                if (start >= end)
                    return BadRequest(ApiResponse<string>.Fail(
                        $"StartAt must be earlier than EndAt for slot '{s.NameSlot}'"
                    ));

                bool duplicateNameInCampus = campus.Slots
                    .Any(existing =>
                        existing.IsActive == true &&
                        existing.NameSlot!.Trim().ToLower() == s.NameSlot.Trim().ToLower());

                if (duplicateNameInCampus)
                    return BadRequest(ApiResponse<string>.Fail(
                        $"Slot name '{s.NameSlot}' already exists in this campus."
                    ));

                bool duplicateNameInBatch = validatedSlots
                    .Any(existing => existing.Name.Trim().ToLower() == s.NameSlot.Trim().ToLower());

                if (duplicateNameInBatch)
                    return BadRequest(ApiResponse<string>.Fail(
                        $"Duplicate slot name '{s.NameSlot}' found in the request."
                    ));

                bool overlapExisting = campus.Slots
                    .Where(existing => existing.IsActive == true)
                    .Any(existing =>
                        start < existing.EndAt && end > existing.StartAt
                    );

                if (overlapExisting)
                    return BadRequest(ApiResponse<string>.Fail(
                        $"Slot '{s.NameSlot}' overlaps with an existing slot in campus."
                    ));

                bool overlapInBatch = validatedSlots.Any(existing =>
                    start < existing.End && end > existing.Start);

                if (overlapInBatch)
                    return BadRequest(ApiResponse<string>.Fail(
                        $"Slot '{s.NameSlot}' overlaps with another slot in the batch."
                    ));

                validatedSlots.Add((s.NameSlot, start, end));
            }

            var createdSlots = new List<SlotCampusDto>();

            foreach (var (name, start, end) in validatedSlots)
            {
                var slot = new Slot
                {
                    NameSlot = name,
                    StartAt = start,
                    EndAt = end,
                    IsActive = true,
                    CampusId = campusId
                };

                _context.Slots.Add(slot);

                createdSlots.Add(new SlotCampusDto
                {
                    NameSlot = slot.NameSlot!,
                    StartAt = slot.StartAt.ToString(),
                    EndAt = slot.EndAt.ToString(),
                });
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<List<SlotCampusDto>>.Success(
                createdSlots,
                $"Created {createdSlots.Count} new slot(s) successfully."
            ));
        }


        [HttpPut("v1/campus/{campusId}slot/{slotId}/active")]
        public async Task<IActionResult> UpdateSlotActiveStatus(int campusId, int slotId, [FromBody] UpdateSlotActiveRequest request)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            if (user == null || user.Role != RoleEnum.Admin.ToString())
                return Unauthorized(ApiResponse<string>.Unauthorized("Only Admin can update slot status"));

            var result = await _campusService.UpdateIsActiveAsync(campusId, slotId, request.IsActive);

            if (result.Status != 200)
                return BadRequest(ApiResponse<string>.Fail(result.Message));

            return Ok(ApiResponse<object>.Success(result.Data!, result.Message));
        }

    }
}
