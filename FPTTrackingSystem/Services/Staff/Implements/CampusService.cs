using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Campus;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.EntityFrameworkCore;
using Repositories.Staff.Implements;
using Repositories.Staff.Interfaces;

namespace FPTTrackingSystem.Services.Staff.Implements
{
    public class CampusService : ICampusService
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

        public async Task<Campus> CreateCampusAsync(CreateCampusDto dto)
        {
            var campus = new Campus
            {
                Name = dto.Name,
                IsActive = true
            };

            return await _campusRepository.AddCampusAsync(campus);
        }

        public async Task<Campus> UpdateCampusAsync(int id, UpdateCampusDto dto)
        {
            var campus = await _campusRepository.GetCampusByIdAsync(id);

            if (campus == null)
                throw new KeyNotFoundException("Campus not found");

            campus.Name = dto.Name;
            campus.IsActive = dto.IsActive;

            await _campusRepository.UpdateCampusAsync(campus);
            return campus;
        }

        public async Task<ApiResponse<List<SlotCampusDto>>> CreateSlotsAsync(
        int campusId,
        List<SlotCreateDto> slots,
        string? userRole)
        {
            if (userRole != RoleEnum.Admin.ToString())
                return ApiResponse<List<SlotCampusDto>>.Unauthorized("Only Admin can create slots");

            if (slots == null || !slots.Any())
                return ApiResponse<List<SlotCampusDto>>.Fail("No slots provided.");

            var campus = await _campusRepository.GetCampusWithSlotsAsync(campusId);

            if (campus == null)
                return ApiResponse<List<SlotCampusDto>>.Fail($"Campus with ID {campusId} not found.");

            var validated = new List<(string Name, TimeOnly Start, TimeOnly End)>();

            foreach (var s in slots)
            {
                if (!TimeOnly.TryParse(s.StartAt, out var start))
                    return ApiResponse<List<SlotCampusDto>>.Fail($"Invalid StartAt format: {s.StartAt}");

                if (!TimeOnly.TryParse(s.EndAt, out var end))
                    return ApiResponse<List<SlotCampusDto>>.Fail($"Invalid EndAt format: {s.EndAt}");

                if (start >= end)
                    return ApiResponse<List<SlotCampusDto>>.Fail(
                        $"StartAt must be earlier than EndAt for slot '{s.NameSlot}'"
                    );

                if (campus.Slots.Any(e =>
                    e.IsActive == true &&
                    e.NameSlot!.Trim().ToLower() == s.NameSlot.Trim().ToLower()))
                {
                    return ApiResponse<List<SlotCampusDto>>.Fail(
                        $"Slot name '{s.NameSlot}' already exists in this campus.");
                }

                if (validated.Any(e =>
                    e.Name.Trim().ToLower() == s.NameSlot.Trim().ToLower()))
                {
                    return ApiResponse<List<SlotCampusDto>>.Fail(
                        $"Duplicate slot name '{s.NameSlot}' found in the request.");
                }

                if (campus.Slots.Any(e =>
                    e.IsActive == true &&
                    start < e.EndAt && end > e.StartAt))
                {
                    return ApiResponse<List<SlotCampusDto>>.Fail(
                        $"Slot '{s.NameSlot}' overlaps with an existing slot.");
                }

                if (validated.Any(e =>
                    start < e.End && end > e.Start))
                {
                    return ApiResponse<List<SlotCampusDto>>.Fail(
                        $"Slot '{s.NameSlot}' overlaps with another slot in the batch.");
                }

                validated.Add((s.NameSlot, start, end));
            }

            var newSlots = validated
                .Select(v => new Slot
                {
                    NameSlot = v.Name,
                    StartAt = v.Start,
                    EndAt = v.End,
                    IsActive = true,
                    CampusId = campusId
                })
                .ToList();

            await _campusRepository.AddSlotsAsync(newSlots);
            await _campusRepository.SaveChangesAsync();

            var response = newSlots.Select(s => new SlotCampusDto
            {
                NameSlot = s.NameSlot!,
                StartAt = s.StartAt.ToString(),
                EndAt = s.EndAt.ToString()
            }).ToList();

            return ApiResponse<List<SlotCampusDto>>.Success(
                response,
                $"Created {response.Count} new slot(s) successfully."
            );
        }
    }
}
