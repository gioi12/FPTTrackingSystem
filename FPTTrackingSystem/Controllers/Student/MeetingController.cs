using DataTranferObjects.Student.Meeting;
using FPTTrackingSystem.Services.Student.Implements;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Student
{
    [Route("api/")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _service;

        public MeetingController(IMeetingService service)
        {
            _service = service;
        }
        /// <summary>
        ///  /api/v1/Student/Meeting/groups/4/schedule/free-time
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("v1/Student/Meeting/groups/{groupId}/schedule/free-time")]
        public async Task<IActionResult> CreateOrUpdateFreeTimeSlots(int groupId, [FromBody] List<FreeTimeSlotRequest> requests)
        {
            try
            {
                await _service.CreateOrUpdateFreeTimeSlotsAsync(groupId, requests);

                return Ok(new
                {
                    status = 200,
                    message = "Free time slots created/updated successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message
                });
            }
        }

        [HttpGet("v1/Student/Meeting/groups/{groupId}/schedule/free-time")]
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.InternalError(ex.Message));
            }
        }

        [HttpPost("v1/Student/Meeting/groups/{groupId}/schedule/finalize")]
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

        [HttpGet("v1/Student/Meeting/schedule/finalize/getById/{GroupId}")]
        public async Task<IActionResult> GetMeetingById(int GroupId)
        {
            try
            {
                var meeting = await _service.GetMeetingByGroupIdAsync(GroupId);
                if (meeting == null)
                    return Ok(ApiResponse<MeetingResponseDTO>.Success(new MeetingResponseDTO(), "Schedule not found."));

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

        [HttpGet("v1/Student/Meeting/group/{groupId}/schedule-dates")]
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

        [HttpPut("v1/Student/Meeting/update-is-meeting/{id}")]
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
