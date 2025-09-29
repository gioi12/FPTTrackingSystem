using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Status
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<TaskAttachment> TaskAttachments { get; set; } = new List<TaskAttachment>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
