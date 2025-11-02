using FPTTrackingSystem.Services.Staff.Implementations;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("v1/Staff/capstone-groups")]
        public async Task<IActionResult> GetCapstoneGroups([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _groupService.GetGroupsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("v1/Staff/capstone-groups/{id}")]
        public async Task<IActionResult> GetGroupById(string id)
        {
            var groupId = int.Parse(id);
            var result = await _groupService.GetGroupByIdAsync(groupId);
            return StatusCode(result.Status, result);
        }

        [HttpGet("v1/Staff/dashboard-majors-groups")]
        public async Task<IActionResult> GetMajorGroupTotals()
        {
            var response = await _groupService.GetMajorGroupTotalsAsync();
            return Ok(response);
        }
        [HttpGet("v1/Staff/group-tracking")]
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


        [HttpPut("v1/Staff/update-role")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateRoleInGroup([FromQuery] int groupId, [FromQuery] int studentId, [FromBody] string newRole)
        {
            var normalizedRole = newRole?.Trim().ToLower();

            if (normalizedRole == "member" || normalizedRole == "student")
            {
                newRole = "student";
            }
            var result = await _groupService.UpdateRoleInGroupAsync(groupId, studentId, newRole);
            return StatusCode(200, result);
        }

        [Authorize(Roles = "Supervisor")]
        [HttpPost("v1/upload/group")]
        public async Task<object> UploadMilestone(IFormFile file, int groupId)
        {
            var message = await _groupService.UploadFileGroup(file, groupId);
            return Ok(ApiResponse<object>.Success(message,"Upload Successfully"));
        }
        [Authorize(Roles = "Supervisor")]
        [HttpDelete("v1/upload/group")]
        public async Task<object> DeleteGroup(int attachmentId)
        {
            await _groupService.DeleteFileGroup(attachmentId);
            return Ok(ApiResponse<object>.Success(null, "Delete attachment successfully."));
        }
        [Authorize]
        [HttpGet("v1/upload/files")]
        public async Task<object> FilesGroup(int groupId)
        {
            var list = await _groupService.GetFilesGroup(groupId);
            return Ok(ApiResponse<object>.Success(list, "Get attachments successfully."));
        }
        [Authorize(Roles = "Staff")]
        [HttpGet("v1/mock-data/group")]
        public async Task<object> MockDataGroup()
        {
            var data = await _groupService.GetMockData();
            return Ok(ApiResponse<object>.Success(data, "Upload Successfully"));
        }
        [Authorize(Roles = "Staff")]
        [HttpPost("v1/mock-data/group")]
        public async Task<object> CreateGroups()
        {
            var message = await _groupService.CreateMockData();
            return Ok(ApiResponse<object>.Success(message, "Upload Successfully"));
        }
    }
}
