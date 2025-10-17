using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Status
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
