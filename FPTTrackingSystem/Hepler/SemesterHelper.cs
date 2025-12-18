using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Semester;
using System.Globalization;

namespace FPTTrackingSystem.Hepler
{
    public class SemesterHelper
    {
        public static List<SemesterWeekDTO> GetWeeks(DateOnly startAt, DateOnly endAt, int semesterId)
        {
            var weeks = new List<SemesterWeekDTO>();

            int weekNumber = 1;
            var currentStart = startAt;

            while (currentStart <= endAt)
            {
                var currentEnd = currentStart.AddDays(6);
                if (currentEnd > endAt)
                    currentEnd = endAt;

                weeks.Add(new SemesterWeekDTO
                {
                    WeekNumber = weekNumber,
                    StartAt = currentStart.ToDateTime(TimeOnly.MinValue),
                    EndAt = currentEnd.ToDateTime(TimeOnly.MinValue),
                    IsVacation = false 
                });

                currentStart = currentStart.AddDays(7);
                weekNumber++;
            }

            return weeks;
        }

        public static DateTime ConvertSolarToLunar(DateTime solarDate)
        {
            var calendar = new ChineseLunisolarCalendar();

            int year = calendar.GetYear(solarDate);
            int month = calendar.GetMonth(solarDate);
            int day = calendar.GetDayOfMonth(solarDate);

            int leapMonth = calendar.GetLeapMonth(year); 

            if (leapMonth > 0 && month >= leapMonth)
            {
                month--; 
            }

            return new DateTime(year, month, day);
        }
    }
}
