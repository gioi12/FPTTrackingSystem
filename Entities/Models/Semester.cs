using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Semester
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartAt { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
}
