using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Semester;

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
                    SemesterId = semesterId,
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
            var calendar = new System.Globalization.ChineseLunisolarCalendar();

            int lunarYear = calendar.GetYear(solarDate);
            int lunarMonth = calendar.GetMonth(solarDate);
            int lunarDay = calendar.GetDayOfMonth(solarDate);

            return new DateTime(lunarYear, lunarMonth, lunarDay);
        }

    }
}
