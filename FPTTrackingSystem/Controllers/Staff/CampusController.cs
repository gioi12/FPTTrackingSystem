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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CampusController : ControllerBase
    {
        private readonly AuthUtils _authUtils;
        private readonly ICampusService _campusService;
        private readonly FpttrackingSystemContext _context;
        public CampusController(ICampusService campusService, AuthUtils authUtils, FpttrackingSystemContext context)
        {
            _campusService = campusService;
            _authUtils = authUtils;
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllCampusesAsync()
        {
            var campuses = await _campusService.GetAllCampusesAsync();
            return Ok(ApiResponse<IEnumerable<CampusDto>>.Success(campuses, "Get all campuses successfully"));
        }

        [HttpGet("ById/{campusId}")]
        public async Task<IActionResult> GetCampusByIdAsync(int campusId)
        {
            var campus = await _campusService.GetByIdWithSlotsAsync(campusId);

            if (campus == null)
                return Ok(ApiResponse<CampusDto>.Success(null, $"Campus with ID {campusId} not found."));

            return Ok(ApiResponse<CampusDto>.Success(campus, "Get campus successfully"));
        }
    }
}
