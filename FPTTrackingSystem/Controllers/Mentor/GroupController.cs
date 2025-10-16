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
                var groups = await _groupService.GetGroupsByUserIdAsync(user.Id ?? 0);

                if (groups == null || groups.Count == 0)
                    return Ok(ApiResponse<object>.Fail("Không tìm thấy nhóm nào cho user này."));

                return Ok(ApiResponse<List<GroupMentorDto>>.Success(groups, "Lấy danh sách nhóm thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }
    }
}
