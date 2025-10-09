using DataTranferObjects.Staff.Group;
using DataTranferObjects.Staff.Semester;

namespace FPTTrackingSystem.Hepler
{
    public class SemesterHelper
    {
        public static List<WeekInfo> GetWeeks(DateOnly startAt, DateOnly endAt, string? semesterBreak = null)
        {
            var weeks = new List<WeekInfo>();
            var breakWeeks = new HashSet<int>();

            // Parse chuỗi "2,5,10" => tuần nghỉ
            if (!string.IsNullOrWhiteSpace(semesterBreak))
            {
                foreach (var part in semesterBreak.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(part.Trim(), out var num))
                        breakWeeks.Add(num);
                }
            }

            int weekNumber = 1;
            var currentStart = startAt;

            while (currentStart <= endAt)
            {
                var currentEnd = currentStart.AddDays(6);
                if (currentEnd > endAt)
                    currentEnd = endAt;

                weeks.Add(new WeekInfo
                {
                    WeekNumber = weekNumber,
                    StartOfWeek = currentStart.ToString("yyyy-MM-dd"),
                    EndOfWeek = currentEnd.ToString("yyyy-MM-dd"),
                    IsVacation = !breakWeeks.Contains(weekNumber)
                });

                currentStart = currentStart.AddDays(7);
                weekNumber++;
            }
            return weeks;
        }

        public static List<WeekInfo> GetSemesterBreakWeeks(List<WeekInfo> allWeeks)
        {
            return allWeeks.FindAll(w => !w.IsVacation);
        }
    }
}
