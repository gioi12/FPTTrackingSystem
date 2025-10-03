using DataTranferObjects.Staff.Request;
using FPTTrackingSystem.Services.Staff;
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
        public async Task<object> GetMilestoneByMarjorAndSemester(int majorId,int semesterId)
        {
            return Ok(await _milestoneService.GetMilestoneByMajorAndSemester(majorId, semesterId));
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("v1/Staff/milestones")]
        public async Task<object> NewMilestone(List<MilestoneCreateRequest> request)
        {
            return Ok(await _milestoneService.CreateMilestoneInSemester(request));
        }
        [Authorize(Roles = "Staff")]
        [HttpPut("v1/Staff/milestones")]
        public async Task<object> updateMilestones(List<MilestoneCreateRequest> request)
        {
            return Ok(await _milestoneService.UpdateInfoMilestone(request));
        }
        [Authorize(Roles = "Staff")]
        [HttpDelete("v1/Staff/milestone/{id}")]
        public async Task<object> deleteMilestone([FromRoute]int id)
        {
            return Ok(await _milestoneService.DeleteMilestone(id));
        }
    }
}
