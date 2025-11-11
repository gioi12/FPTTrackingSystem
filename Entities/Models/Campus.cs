using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Campus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
