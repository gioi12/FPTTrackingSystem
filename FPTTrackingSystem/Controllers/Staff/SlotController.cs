using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Campus;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FPTTrackingSystem.Services.Staff.Interfaces.ICampusService;

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

        [HttpGet("v1/slot/ById/{campusId}")]
        public async Task<IActionResult> GetCampusByIdAsync(int campusId)
        {
            var campus = await _campusService.GetByIdWithSlotsAsync(campusId);

            if (campus == null)
                return Ok(ApiResponse<CampusDto>.Success(null, $"Campus with ID {campusId} not found."));

            return Ok(ApiResponse<CampusDto>.Success(campus, "Get campus successfully"));
        }

        [HttpPost("v1/slot/{campusId}")]
        public async Task<ActionResult> CreateSlots(int campusId,[FromBody] List<SlotCreateDto> slots)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var userRole = user?.Role;

            var result = await _campusService.CreateSlotsAsync(campusId, slots, userRole);

            if (result.Status != 200)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("v1/campus/{campusId}/slot/{slotId}/active")]
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
