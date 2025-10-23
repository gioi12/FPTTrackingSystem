using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Student.Meeting
{
    public class MeetingMinuteRequest
    {
        [Required]
        public int MeetingId { get; set; }
        [Required]

        public DateTime? MeetingMinusDate { get; set; }
        [Required]
        public string? Attendance { get; set; }

        public string? Issue { get; set; }
        [Required]
        public string? MeetingContent { get; set; }

        public string? Other { get; set; }

    }
}
