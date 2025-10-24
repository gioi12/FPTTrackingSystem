using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Staff.Implementations;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Staff
{
    [Route("api/")]
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
        public async Task<object> GetMilestoneByMarjorAndSemester(int majorCateId, int semesterId)
        {
            return Ok(await _deliverableSevice.GetDeliverableByCodeAndSemester(semesterId, majorCateId));
        }
        [Authorize]
        [HttpPost("v1/upload/milestone")]
        public async Task<object> UploadMilestone(IFormFile file, int groupId, int deliveryItemId)
        {
            return Ok(await _deliverableSevice.UploadFileMilestoneItem(file, groupId, deliveryItemId));
        }
        [Authorize]
        [HttpGet("v1/deliverables/group/{id}")]
        public async Task<object> GetMilestoneByGroupId([FromRoute] int id)
        {
            return Ok(await _deliverableSevice.GetDeliverableByGroupId(id));
        }
        [Authorize]
        [HttpGet("v1/deliverables/group/detail")]
        public async Task<object> GetMilestoneDetail(int groupdId, int deliverableId)
        {
            return Ok(await _deliverableSevice.GetDeliverableByIdAndGroupId(groupdId, deliverableId));
        }
        [Authorize]
        [HttpPut("v1/deliverables/confirmed")]
        public async Task<object> ConfirmDelivery(int groupdId, int deliverableId,string? note)
        {
            return Ok(await _deliverableSevice.ConfirmDeliverable(groupdId, deliverableId,note));
        }

        [HttpGet("v1/deliverables/getByGroupId/{id}")]
        public async Task<IActionResult> GetDeliverableGroupById(int id)
        {
            try
            {
                var result = await _deliverableSevice.GetDeliverableGroupsByGroupIdAsync(id);

                if (result == null)
                    return Ok(ApiResponse<object>.Success(null, "Không tìm thấy DeliverableGroup."));

                return Ok(ApiResponse<List<DeliverableGroupDetailDTO>>.Success(result, "Lấy DeliverableGroup thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail($"Lỗi: {ex.Message}"));
            }
        }
        [Authorize]
        [HttpDelete("v1/upload/milestone")]
        public async Task<object> DeleteMilestone(int attachmentId)
        {
            await _deliverableSevice.DeleteFileMilestoneItem(attachmentId);
            return Ok(ApiResponse<object>.Success(null, "Delete attachment successfully."));
        }
        [Authorize]
        [HttpPut("v1/deliverables/reject")]
        public async Task<object> RejectDelivery(int groupdId, int deliverableId, string? note)
        {
            var result = await _deliverableSevice.RejectDeliverable(groupdId, deliverableId, note);
            return Ok(ApiResponse<object>.Success(result, "Delete attachment successfully."));

        }
        [Authorize(Roles = "Supervisor")]
        [HttpPut("v1/deliverables/Mark-download")]
        public async Task<object> MarkDownload(int attachmentId)
        {
            await _deliverableSevice.MarkDownload(attachmentId);
            return Ok(ApiResponse<object>.Success(null, "Mark attachment successfully."));
        }
    }
}
