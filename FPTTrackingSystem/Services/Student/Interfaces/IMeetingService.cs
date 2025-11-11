using DataTranferObjects.Student.Meeting;

using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Student.Interfaces
{
    public interface IMeetingService
    {
        Task<GroupFreeTimeDto> GetFreeTimeSlotsByGroupIdAsync(int groupId);
        Task<FinalizeScheduleResponseDto> FinalizeScheduleAsync(int groupId, FinalizeScheduleRequestDto dto);
        Task<MeetingMinuteRes?> CreateMeetingMinute(MeetingMinuteRequest request);
        Task<MeetingMinuteRes> GetMeetingMinuteDate(int meetingDateId);

        Task<MeetingMinuteRes> UpdateMeetingMinute(MeetingMinuteUpdateReq req);
        System.Threading.Tasks.Task CreateOrUpdateFreeTimeSlotsAsync(int groupId, List<FreeTimeSlotRequest> requests);
        System.Threading.Tasks.Task DeleteMeetingMinute(int id);
        Task<MeetingResponseDTO?> GetMeetingByGroupIdAsync(int meetingId);
        Task<ApiResponse<List<MeetingScheduleDateDetailDto>>> GetMeetingScheduleDatesByGroupIdAsync(int groupId);
        Task<bool> UpdateIsMeetingAsync(int id, bool isMeeting);
    }
}
