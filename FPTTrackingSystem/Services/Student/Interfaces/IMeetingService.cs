using DataTranferObjects.Student.Meeting;
using Entities.Models;

namespace FPTTrackingSystem.Services.Student.Interfaces
{
    public interface IMeetingService
    {
        Task<MeetingMinute> CreateMeetingMinute(MeetingMinuteRequest request);
        Task<MeetingMinute> GetMeetingMinute(int meetingId);

    }
}
