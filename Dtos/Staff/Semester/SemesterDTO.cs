using DataTranferObjects.Staff.Group;
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
        public string? Description { get; set; }
        public List<SemesterWeekDTO> Weeks { get; set; } = new(); 
        public bool? IsActive { get; set; }
        public List<SemesterWeekDTO> SemesterBreak { get; set; } = new();
    }

    public class SemesterWeekDTO
    {
        public int? WeekNumber { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool? IsVacation { get; set; }
    }

    public class SemesterWeekUpdateDTO
    {
        public int? SemesterId { get; set; }

        public int? WeekNumber { get; set; }

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public bool? IsVacation { get; set; }
    }

    public class UpdateVacationWeeksRequest
    {
        public int SemesterId { get; set; }
        public List<WeekUpdateDto> Weeks { get; set; } = new();
    }

    public class WeekUpdateDto
    {
        public int WeekNumber { get; set; }
        public bool IsVacation { get; set; }
    }


}
