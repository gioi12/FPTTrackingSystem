using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Meeting
{
    public int Int { get; set; }

    public string? Date { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<MeetingMinute> MeetingMinutes { get; set; } = new List<MeetingMinute>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
