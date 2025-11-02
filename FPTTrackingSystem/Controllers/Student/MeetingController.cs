using DataTranferObjects.Student.Meeting;
using FPTTrackingSystem.Services.Student.Implements;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Wrappers;
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
                return StatusCode(200, ApiResponse<object>.Success(result, "Free time slots updated successfully.", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message, 403));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpGet("groups/{groupId}/schedule/free-time")]
        public async Task<IActionResult> GetFreeTimeSlots(int groupId)
        {
            try
            {
                var result = await _service.GetFreeTimeSlotsByGroupIdAsync(groupId);
                return Ok(ApiResponse<object>.Success(result, "Retrieved student free time successfully.", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message, 403));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpPost("groups/{groupId}/schedule/finalize")]
        public async Task<IActionResult> FinalizeSchedule(int groupId, [FromBody] FinalizeScheduleRequestDto request)
        {

            try
            {
                var result = await _service.FinalizeScheduleAsync(groupId, request);
                return Ok(ApiResponse<object>.Success(result, "Schedule finalized successfully.", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message, 403));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpGet("schedule/finalize/getById/{GroupId}")]
        public async Task<IActionResult> GetMeetingById(int GroupId)
        {
            try
            {
                var meeting = await _service.GetMeetingByGroupIdAsync(GroupId);
                if (meeting == null)
                    return Ok(ApiResponse<object>.Success(new object(), "Meeting not found."));

                return Ok(ApiResponse<object>.Success(meeting, "Meeting retrieved successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message, 403));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpGet("group/{groupId}/schedule-dates")]
        public async Task<IActionResult> GetMeetingScheduleDatesByGroupId(int groupId)
        {
            try
            {
                var response = await _service.GetMeetingScheduleDatesByGroupIdAsync(groupId);
                return StatusCode(response.Status, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message, 403));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpPut("update-is-meeting/{id}")]
        public async Task<IActionResult> UpdateIsMeetingAsync(int id, [FromBody] bool isMeeting)
        {
            try
            {
                var success = await _service.UpdateIsMeetingAsync(id, isMeeting);
                if (!success)
                    return StatusCode(404, ApiResponse<object>.Fail("Meeting schedule not found.", 404));

                return Ok(ApiResponse<object>.Success(true, "IsMeeting updated successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message, 403));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }
    }
}
