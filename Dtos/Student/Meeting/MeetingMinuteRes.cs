using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Student.Meeting
{
    public class MeetingMinuteRes
    {
        public int Id { get; set; }

        public DateTime? startAt { get; set; }
        public DateTime? endAt { get; set; }

        public string? CreateBy { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? Attendance { get; set; }

        public string? Issue { get; set; }

        public string? MeetingContent { get; set; }

        public string? Other { get; set; }
    }
}
