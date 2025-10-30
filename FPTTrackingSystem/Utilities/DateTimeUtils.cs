using Entities.Models;
using System.Globalization;

namespace FPTTrackingSystem.Utilities
{
    public class DateTimeUtils
    {
        // ham nay tinh theo tuan
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
        public static DateTime GetTargetDate(
            string data,
            DateTime startDate,
            List<(DateTime startAt, DateTime endAt)> holidays)
        {
            var parts = data.Split('-', StringSplitOptions.TrimEntries);
            if (parts.Length != 3)
                throw new ArgumentException("Invalid format. Expected: 'Week X - Day - HH:mm'");

            if (!int.TryParse(parts[0].Replace("Week", "").Trim(), out int week))
                throw new ArgumentException("Invalid week format");

            if (!Enum.TryParse<DayOfWeek>(parts[1].Trim(), true, out var dayOfWeek))
                throw new ArgumentException("Invalid day format");

            if (!TimeSpan.TryParseExact(parts[2].Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out var time))
                throw new ArgumentException("Invalid time format");

            // Tính ngày gốc chưa tính nghỉ
            var result = startDate.AddDays((week - 1) * 7);
            while (result.DayOfWeek != dayOfWeek)
                result = result.AddDays(1);
            result = result.Date.Add(time);

            // Tính tổng số ngày nghỉ xảy ra TRƯỚC result
            double extraDays = 0;

            foreach (var (startAt, endAt) in holidays)
            {
                if (endAt < startDate || startAt >= result)
                    continue;

                var overlapStart = startAt > startDate ? startAt : startDate;
                var overlapEnd = endAt < result ? endAt : result;
                extraDays += (overlapEnd - overlapStart).TotalDays;
            }

            return result.AddDays(extraDays);
        }

        private static DateTime GetDayInWeek(DateTime startDate, DayOfWeek targetDay)
        {
            int diff = targetDay - startDate.DayOfWeek;
            if (diff < 0) diff += 7;
            return startDate.AddDays(diff);
        }

    }
}
