using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Meeting
{
    public int Id { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? MeetingLink { get; set; }

    public string? Time { get; set; }

    public string? DayOfWeek { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<MeetingScheduleDate> MeetingScheduleDates { get; set; } = new List<MeetingScheduleDate>();
}
