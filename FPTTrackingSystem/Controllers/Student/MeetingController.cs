using DataTranferObjects.Student.Meeting;
using FPTTrackingSystem.Services.Student.Implements;
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

        [HttpGet("groups/{groupId}/schedule/free-time")]
        public async Task<IActionResult> GetFreeTimeSlots(int groupId)
        {
            var result = await _service.GetFreeTimeSlotsByGroupIdAsync(groupId);
            return Ok(new
            {
                success = 200,
                message = "Lấy thành công thời gian rảnh của sinh viên",
                data = result
            });
        }

        [HttpPost("groups/{groupId}/schedule/finalize")]
        public async Task<IActionResult> FinalizeSchedule(int groupId, [FromBody] FinalizeScheduleRequestDto request)
        {

            var result = await _service.FinalizeScheduleAsync(groupId, request);

            return Ok(new
            {
                success = 200,
                message = "Schedule finalized successfully",
                data = result
            });
        }

        [HttpGet("schedule/finalize/getById/{id}")]
        public async Task<IActionResult> GetMeetingById(int id)
        {
            var meeting = await _service.GetMeetingByIdAsync(id);
            if (meeting == null)
                return NotFound(new { message = "Không tìm thấy cuộc họp" });

            return Ok(meeting);
        }
    }
}
