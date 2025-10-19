using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/v1/Staff")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("capstone-groups")]
        public async Task<IActionResult> GetCapstoneGroups([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _groupService.GetGroupsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("capstone-groups/{id}")]
        public async Task<IActionResult> GetGroupById(string id)
        {
            var groupId = int.Parse(id);
            var result = await _groupService.GetGroupByIdAsync(groupId);
            return StatusCode(result.Status, result);
        }

        [HttpGet("dashboard-majors-groups")]
        public async Task<IActionResult> GetMajorGroupTotals()
        {
            var response = await _groupService.GetMajorGroupTotalsAsync();
            return Ok(response);
        }
        [HttpGet("group-tracking")]
        public async Task<IActionResult> GetGroupTracking(
    [FromQuery] string groupId,
    [FromQuery] string startDate,
    [FromQuery] string endDate)
        {
            if (!int.TryParse(groupId, out int gId))
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "GroupId không hợp lệ. Phải là số nguyên.",
                    data = (object?)null
                });
            }

            if (!DateTime.TryParseExact(startDate, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime sDate))
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "StartDate không hợp lệ. Định dạng phải là dd/MM/yyyy.",
                    data = (object?)null
                });
            }

            if (!DateTime.TryParseExact(endDate, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime eDate))
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "EndDate không hợp lệ. Định dạng phải là dd/MM/yyyy.",
                    data = (object?)null
                });
            }

            var result = await _groupService.GetGroupTrackingAsync(gId, sDate, eDate);
            return StatusCode(result.Status, result);
        }


        [HttpPost("update-role")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateRoleInGroup([FromQuery] int groupId, [FromQuery] int studentId, [FromBody] string newRole)
        {
            var result = await _groupService.UpdateRoleInGroupAsync(groupId, studentId, newRole);
            return StatusCode(200, result);
        }


    }
}
