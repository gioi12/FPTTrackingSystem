using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MeetingMinute
{
    public int Id { get; set; }

    public DateTime? MeetingMinusDate { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? CreateAt { get; set; }

    public string? Attendance { get; set; }

    public string? Issue { get; set; }

    public string? MeetingContent { get; set; }

    public string? Other { get; set; }

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
