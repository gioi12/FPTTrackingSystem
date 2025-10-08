using DataTranferObjects.Staff.Semester;

namespace FPTTrackingSystem.Hepler
{
    public class SemesterHelper
    {
        public static List<WeekInfo> GetWeeks(DateOnly start, DateOnly end)
        {
            var result = new List<WeekInfo>();
            int weekNumber = 1;

            var currentStart = start;

            while (currentStart <= end)
            {
                var currentEnd = currentStart.AddDays(6);
                if (currentEnd > end) currentEnd = end;

                result.Add(new WeekInfo
                {
                    WeekNumber = weekNumber++,
                    StartOfWeek = currentStart,
                    EndOfWeek = currentEnd
                });

                currentStart = currentStart.AddDays(7);
            }

            return result;
        }
    }
}
