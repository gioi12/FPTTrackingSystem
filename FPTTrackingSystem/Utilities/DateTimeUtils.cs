using Entities.Models;
using System.Globalization;

namespace FPTTrackingSystem.Utilities
{
    public class DateTimeUtils
    {
        public static DateTime? GetDeadlineDate(string deadline, List<SemesterWeek> weeks)
        {
            if (string.IsNullOrWhiteSpace(deadline) || weeks == null || weeks.Count == 0)
                return null;

            try
            {
                deadline = deadline.Trim();
                var parts = deadline
                    .Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToArray();
                if (parts.Length < 4)
                    return null;

                int weekNum = int.Parse(parts[1]);

                string dayName = parts[2];

                string timePart = parts[3];

                var week = weeks.FirstOrDefault(w => w.WeekLearn == weekNum || w.WeekNumber == weekNum);
                if (week?.StartAt == null)
                    return null;

                if (!Enum.TryParse<DayOfWeek>(dayName, true, out var dayOfWeek))
                    return null;

                DateTime start = week.StartAt.Value.Date;

                DateTime targetDay = GetDayInWeek(start, dayOfWeek);

                if (TimeSpan.TryParseExact(timePart, new[] { "hh\\:mm", "hh\\:mm\\:ss" }, CultureInfo.InvariantCulture, out var time))
                    targetDay = targetDay.Date.Add(time);

                return targetDay;
            }
            catch
            {
                return null;
            }
        }
        private static DateTime GetDayInWeek(DateTime startDate, DayOfWeek targetDay)
        {
            int diff = targetDay - startDate.DayOfWeek;
            if (diff < 0) diff += 7;
            return startDate.AddDays(diff);
        }

    }
}
