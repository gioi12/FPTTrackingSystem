using DataTranferObjects.Student.Meeting;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Student
{
    [Route("api/")]
    [ApiController]
    public class MeetingMinuteController : ControllerBase
    {
        private readonly IMeetingService _meetingService;
        public MeetingMinuteController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }
        [HttpGet("v1/MeetingMinute")]
        [Authorize]
        public async Task<object> GetMeetingMinuteByMeetingId([FromQuery]int meetingDateId)
        {
            return Ok(ApiResponse<object>.Success( await _meetingService.GetMeetingMinuteDate(meetingDateId), "Find meeting minute success."));
        }

        [HttpPost("v1/MeetingMinute")]
        [Authorize(Roles = "Student")]
        public async Task<object> CreateMeetingMinute([FromBody] MeetingMinuteRequest request)
        {
            return Ok(ApiResponse<object>.Success(await _meetingService.CreateMeetingMinute(request), "Create meeting minute success."));
        }

        [HttpPut("v1/MeetingMinute")]
        [Authorize(Roles = "Student")]
        public async Task<object> UpdateMeetingMinuteBy([FromBody] MeetingMinuteUpdateReq request)
        {
            return Ok(ApiResponse<object>.Success(await _meetingService.UpdateMeetingMinute(request), "Update meeting minute success."));
        }

        [HttpDelete("v1/MeetingMinute/{id}")]
        [Authorize(Roles = "Student")]
        public async Task<object> DeleteMeetingMinuteBy(int id)
        {
            await _meetingService.DeleteMeetingMinute(id);
            return Ok(ApiResponse<object>.Success(null, "Update meeting minute success."));
        }

        [HttpGet("v1/MeetingMinute/attendance")]
        [Authorize]
        public async Task<object> GetListAttens([FromQuery] int groupId)
        {
            return Ok(ApiResponse<object>.Success(await _meetingService.GetMeetingAttendances(groupId), "Find atts success."));
        }
    }
}
