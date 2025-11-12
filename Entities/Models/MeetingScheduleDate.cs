using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MeetingScheduleDate
{
    public int Id { get; set; }

    public int? MeetingId { get; set; }

    public DateTime? MeetingDate { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsMeeting { get; set; }

    public int? SlotId { get; set; }

    public virtual Meeting? Meeting { get; set; }

    public virtual MeetingMinute? MeetingMinute { get; set; }

    public virtual Slot? Slot { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
