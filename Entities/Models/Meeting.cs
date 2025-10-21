using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Meeting
{
    public int Int { get; set; }

    public string? Date { get; set; }

    public virtual ICollection<MeetingMinute> MeetingMinutes { get; set; } = new List<MeetingMinute>();
}
