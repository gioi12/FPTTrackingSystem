using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MajorCategory
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? IsActive { get; set; }

    public int? MajorId { get; set; }

    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Major? Major { get; set; }

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
