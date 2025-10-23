
using DataTranferObjects.Staff.Response;
using DataTranferObjects.Student.Meeting;
using Entities.Models;
using Mapster;

namespace FPTTrackingSystem.Mappers
{
    public class MeetingMinuteMapper
    {
        public static void ToMeetingMinuteResponse()
        {
            TypeAdapterConfig<MeetingMinute, MeetingMinuteRes>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.MeetingMinusDate, src => src.MeetingMinusDate)
                .Map(dest => dest.CreateAt, src => src.CreateAt)
                .Map(dest => dest.CreateBy, src => src.CreateByNavigation != null ? src.CreateByNavigation.Fullname : null)
                .Map(dest => dest.Attendance, src => src.Attendance)
                .Map(dest => dest.Issue, src => src.Issue)
                .Map(dest => dest.MeetingContent, src => src.MeetingContent)
                .Map(dest => dest.Other, src => src.Other);
        }
    }
}
