using DataTranferObjects.Staff.Group;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Mentor
{
    [Route("api/v1/Mentor/")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly AuthUtils _authUtils;

        public GroupController(IGroupService groupService, AuthUtils authUtils)
        {
            _groupService = groupService;
            _authUtils = authUtils;
        }

        [HttpGet("getGroups")]
        public async Task<IActionResult> GetGroupsByUserId()
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            try
            {
                var groupsResponse = await _groupService.GetGroupsByUserIdAsync(user.Id ?? 0);

                var groups = groupsResponse.Data;

                if (groups == null || groups.Count == 0)
                    return Ok(ApiResponse<object>.Success(null, "Không tìm thấy nhóm nào cho mentor này."));

                return Ok(ApiResponse<List<GroupMentorDto>>.Success(groups, "Lấy danh sách nhóm thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }

        [HttpGet("expired-groups")]
        public async Task<IActionResult> GetExpiredGroups([FromQuery] int semesterId)
        {
            // Lấy thông tin user từ token
            var user = await _authUtils.GetUserInfoFromCookie();

            if (user == null)
                return Unauthorized(new { message = "User not found!" });

            if (user.Role != "Supervisor" && user.Role != "SupervisorHead")
                return Forbid("Only supervisors can access this!");

            var result = await _groupService.GetExpiredGroupsBySupervisorAsync(user.Id ?? 0, semesterId);

            return Ok(result);
        }

    }
}
