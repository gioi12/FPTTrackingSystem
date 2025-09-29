using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Major
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
