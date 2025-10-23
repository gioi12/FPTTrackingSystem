using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Student.Meeting
{
    public class MeetingDTO
    {
    }

    public class FreeTimeSlotDto
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public List<string> TimeSlots { get; set; } = new();
    }

    public class FreeTimeSlotsRequest
    {
        public List<FreeTimeSlotDto> FreeTimeSlots { get; set; } = new();
    }

    public class FinalizeScheduleRequestDto
    {
        public FinalMeetingDto FinalMeeting { get; set; } = new FinalMeetingDto();
    }

    public class FinalMeetingDto
    {
        public string Day { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string MeetingLink { get; set; } = string.Empty;
    }

    public class FinalizeScheduleResponseDto
    {
        public bool IsFinalized { get; set; }
        public FinalMeetingInfo FinalMeeting { get; set; } = new FinalMeetingInfo();
    }

    public class FinalMeetingInfo
    {
        public int Id { get; set; }
        public bool IsFinalized { get; set; }
        public string Day { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string MeetingLink { get; set; } = string.Empty;
        public DateTime FinalizedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }



}
