using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Helper
{
    public class CaculateDate
    {
        public List<DateTime> GetAllDatesForDayOfWeek(DateTime start, DateTime end, string dayOfWeek)
        {
            var targetDay = Enum.Parse<DayOfWeek>(dayOfWeek, true);
            var dates = new List<DateTime>();

            var current = start;
            while (current.DayOfWeek != targetDay)
                current = current.AddDays(1);

            while (current <= end)
            {
                dates.Add(current);
                current = current.AddDays(7);
            }

            return dates;
        }

        public int GetWeekNumberInSemester(DateTime semesterStart, DateTime date)
        {
            var diff = (date - semesterStart).Days;
            return (diff / 7) + 1;
        }

    }
}
