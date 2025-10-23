using DataTranferObjects.Student.Meeting;

using DataTranferObjects.Student.Meeting;
using Entities.Models;

namespace FPTTrackingSystem.Services.Student.Interfaces
{
    public interface IMeetingService
    {
        Task<object> CreateOrUpdateFreeTimeSlotsAsync(int groupId, FreeTimeSlotsRequest request);
        Task<List<StudentFreeTimeDto>> GetFreeTimeSlotsByGroupIdAsync(int groupId);
        Task<FinalizeScheduleResponseDto> FinalizeScheduleAsync(int groupId, FinalizeScheduleRequestDto dto);
        Task<MeetingMinuteRes?> CreateMeetingMinute(MeetingMinuteRequest request);
        Task<MeetingMinuteRes> GetMeetingMinute(int meetingId);

        Task<MeetingMinuteRes> UpdateMeetingMinute(MeetingMinuteUpdateReq req);

        System.Threading.Tasks.Task DeleteMeetingMinute(int id);
        Task<MeetingResponseDTO?> GetMeetingByIdAsync(int meetingId);
    }
}
