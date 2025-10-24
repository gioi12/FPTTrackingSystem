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
        public List<DayFreeTimeSlot> TimeSlots { get; set; } = new();
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

    public class StudentFreeTimeDto
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public List<FreeTimeSlotByDayDto> FreeTimeSlots { get; set; }
    }

    public class FreeTimeSlotByDayDto
    {
        public string DayOfWeek { get; set; }
        public List<string> TimeSlots { get; set; }
    }

    public class StudentFreeTimeSlot
    {
        public int StudentId { get; set; }
        public List<DayFreeTimeSlot> TimeSlotsByDay { get; set; }
    }

    public class DayFreeTimeSlot
    {
        public string DayOfWeek { get; set; }
        public List<string> TimeSlots { get; set; }
    }

    public class MeetingResponseDTO
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? MeetingLink { get; set; }
        public string? Time { get; set; }
        public string? DayOfWeek { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class MeetingScheduleDateDetailDto
    {
        public int Id { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string? Description { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? MeetingLink { get; set; }
        public string? Time { get; set; }
        public string? DayOfWeek { get; set; }
    }

}
