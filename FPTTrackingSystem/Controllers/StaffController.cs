using FPTTrackingSystem.Services.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public StaffController(IGroupService groupService)
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

    }
}
