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

    public class FreeTimeSlotRequest
    {
        public List<int> Slots { get; set; } = new();
        public string DayOfWeek { get; set; } = string.Empty;
    }

    public class FreeTimeSlotsDto
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public List<int> SlotIds { get; set; }
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
        public int SlotId { get; set; }
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
        public SlotInfo Slot { get; set; }
        public string MeetingLink { get; set; } = string.Empty;
        public DateTime FinalizedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SlotInfo
    {
        public int Id { get; set; }
        public string NameSlot { get; set; } = string.Empty;
        public TimeOnly? StartAt { get; set; }
        public TimeOnly? EndAt { get; set; }
    }

    public class GroupFreeTimeDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public List<StudentFreeTimeDto> Students { get; set; }
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
        public List<TimeSlotDto> TimeSlots { get; set; }
    }

    public class TimeSlotDto
    {
        public int Id { get; set; }
        public string NameSlot { get; set; } = string.Empty;
        public TimeOnly StartAt { get; set; }
        public TimeOnly EndAt { get; set; }
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
        public SlotInfo? Slot { get; set; }
        public string? DayOfWeek { get; set; }
        public string? CreatedByName { get; set; }
    }

    public class MeetingScheduleDateDetailDto
    {
        public int Id { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string? Description { get; set; }
        public bool? IsMeeting { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? MeetingLink { get; set; }
        public bool? IsActive { get; set; }
        public TimeOnly? StartAt { get; set; }
        public TimeOnly? EndAt { get; set; }
        public string? DayOfWeek { get; set; }
        public bool? IsMinute { get; set; } = false;

    }

    public class UpdateMeetingScheduleDateDto
    {
        public DateTime? MeetingDate { get; set; }
        public string? StartAt { get; set; }       
        public string? EndAt { get; set; }
        public bool? IsActive { get; set; }
        public string? Description { get; set; }
    }


}
