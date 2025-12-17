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

            if (!Enum.TryParse(parts[1].Trim(), true, out DayOfWeek dayOfWeek))
                throw new ArgumentException("Invalid day format");

            if (!TimeSpan.TryParseExact(parts[2].Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out var time))
                throw new ArgumentException("Invalid time format");

            // Bước 1: Tính ngày mục tiêu ban đầu (chưa xét ngày nghỉ)
            var result = startDate.AddDays((week - 1) * 7);

            // Bước 2: Tìm đúng ngày trong tuần
            while (result.DayOfWeek != dayOfWeek)
                result = result.AddDays(1);

            // Bước 3: Cộng thêm số ngày nghỉ nằm GIỮA startDate và result
            int totalHolidayDays = 0;
            foreach (var (holidayStart, holidayEnd) in holidays.OrderBy(h => h.startAt))
            {
                // Chỉ tính ngày nghỉ nằm trong khoảng [startDate, result]
                if (holidayEnd <= startDate || holidayStart >= result)
                    continue; // Kỳ nghỉ nằm ngoài phạm vi

                var overlapStart = holidayStart > startDate ? holidayStart : startDate;
                var overlapEnd = holidayEnd < result ? holidayEnd : result;

                totalHolidayDays += (int)(overlapEnd - overlapStart).TotalDays;
            }

            // Cộng thêm số ngày nghỉ vào kết quả
            result = result.AddDays(totalHolidayDays);

            // Bước 4: Nếu kết quả cuối cùng rơi vào kỳ nghỉ, đẩy ra sau kỳ nghỉ
            bool adjusted;
            do
            {
                adjusted = false;
                foreach (var (holidayStart, holidayEnd) in holidays.OrderBy(h => h.startAt))
                {
                    if (result >= holidayStart && result < holidayEnd)
                    {
                        result = holidayEnd;
                        adjusted = true;
                        break;
                    }
                }
            } while (adjusted);

            // Bước 5: Đảm bảo vẫn đúng ngày trong tuần sau khi điều chỉnh
            while (result.DayOfWeek != dayOfWeek)
                result = result.AddDays(1);

            // Bước 6: Thêm giờ phút
            result = result.Date.Add(time);

            return result;
        }

        private static DateTime GetDayInWeek(DateTime startDate, DayOfWeek targetDay)
        {
            int diff = targetDay - startDate.DayOfWeek;
            if (diff < 0) diff += 7;
            return startDate.AddDays(diff);
        }

    }
}
