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

}
