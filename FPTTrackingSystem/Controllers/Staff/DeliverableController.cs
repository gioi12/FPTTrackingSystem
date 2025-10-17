using Entities.Models;
using FPTTrackingSystem.Services.Staff.Implementations;
using FPTTrackingSystem.Services.Staff.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliverableController : ControllerBase
    {
        private readonly IDeliverableSevice _deliverableSevice;
        public DeliverableController(IDeliverableSevice delivery)
        {
            _deliverableSevice = delivery;
        }

        [Authorize]
        [HttpGet("v1/Staff/deliverables")]
        public async Task<object> GetMilestoneByMarjorAndSemester(int majorCateId,int semesterId)
        {
            return Ok(await _deliverableSevice.GetDeliverableByCodeAndSemester(semesterId, majorCateId));
        }
        [Authorize]
        [HttpPost("v1/upload/milestone")]
        public async Task<object> UploadMilestone(IFormFile file,int groupId,int deliverableId)
        {
            return Ok(await _deliverableSevice.UploadFileMilestone(file, groupId, deliverableId));
        }
    }
}
