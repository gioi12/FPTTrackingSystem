using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Meeting
{
    public int Id { get; set; }

    public DateTime? MeetingDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<MeetingMinute> MeetingMinutes { get; set; } = new List<MeetingMinute>();
}
