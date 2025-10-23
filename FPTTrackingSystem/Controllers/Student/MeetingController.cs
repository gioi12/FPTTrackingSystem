using DataTranferObjects.Student.Meeting;
using FPTTrackingSystem.Services.Student.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Student
{
    [Route("api/v1/Student/[controller]/")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _service;

        public MeetingController(IMeetingService service)
        {
            _service = service;
        }

        [HttpPost("groups/{groupId}/schedule/free-time")]
        public async Task<IActionResult> CreateOrUpdateFreeTimeSlots(int groupId, [FromBody] FreeTimeSlotsRequest request)
        {
            try
            {
                var result = await _service.CreateOrUpdateFreeTimeSlotsAsync(groupId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("/groups/{groupId}/schedule/free-time")]
        public async Task<IActionResult> GetFreeTimeSlots(int groupId)
        {
            var result = await _service.GetFreeTimeSlotsByGroupIdAsync(groupId);
            return Ok(new
            {
                success = true,
                message = "Lấy thành công thời gian rảnh của sinh viên",
                data = result
            });
        }
    }
}
