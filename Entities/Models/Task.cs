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

    public int StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? PriorityId { get; set; }

    public string? Process { get; set; }

    public int? MilestoneId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Milestone? Milestone { get; set; }

    public virtual Priority? Priority { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<TaskUser> TaskUsers { get; set; } = new List<TaskUser>();
}
