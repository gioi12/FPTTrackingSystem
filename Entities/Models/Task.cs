using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Task
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime Deadline { get; set; }

    public string? Type { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? Priority { get; set; }

    public string? Process { get; set; }

    public int? DeliverableId { get; set; }

    public bool? IsActive { get; set; }

    public int? MeetingScheduleDateId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Deliverable? Deliverable { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual MeetingScheduleDate? MeetingScheduleDate { get; set; }

    public virtual ICollection<TaskUser> TaskUsers { get; set; } = new List<TaskUser>();

    public virtual ICollection<Task> TaskReferences { get; set; } = new List<Task>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
