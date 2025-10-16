using DataTranferObjects.Staff.Request;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Admin
{
    [Route("api/")]
    [ApiController]
    public class MilestoneController : ControllerBase
    {
        private readonly IMilestoneService _milestoneService;
        public MilestoneController(IMilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("v1/Staff/milestones")]
        public async Task<object> GetMilestoneByMarjorAndSemester(int majorCateId)
        {
            return Ok(await _milestoneService.GetMilestonesByMajor(majorCateId));
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("v1/Staff/milestones")]
        public async Task<object> NewMilestone(List<MilestoneCreateRequest> request)
        {
            return Ok(await _milestoneService.CreateMilestoneInSemester(request));
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("v1/Staff/milestones")]
        public async Task<object> updateMilestones(MilestoneUpdateRequest request)
        {
            return Ok(await _milestoneService.UpdateInfoMilestone(request));
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("v1/Staff/milestone/{id}")]
        public async Task<object> deleteMilestone([FromRoute]int id)
        {
            return Ok(await _milestoneService.DeleteMilestone(id));
        }

        [HttpGet("v1/Student/milestone/group/{groupId}")]
        public async Task<IActionResult> GetMilestonesByGroupId(int groupId)
        {
            try
            {
                var milestones = await _milestoneService.GetMilestonesByGroupIdAsync(groupId);

                if (milestones == null || milestones.Count == 0)
                    return Ok(ApiResponse<object>.Fail("Không tìm thấy milestone nào cho group này."));

                return Ok(ApiResponse<List<MilestonesDTO>>.Success(milestones, "Lấy danh sách milestone thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }
    }
}
