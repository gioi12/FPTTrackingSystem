using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Semester
{
    public class SemesterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<WeekInfo> Weeks { get; set; } = new();
    }

    public class WeekInfo
    {
        public int WeekNumber { get; set; }
        public DateOnly StartOfWeek { get; set; }
        public DateOnly EndOfWeek { get; set; }
    }
}
