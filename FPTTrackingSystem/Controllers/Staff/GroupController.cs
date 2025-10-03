using FPTTrackingSystem.Services.Staff;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/Staff")]
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
    }
}
