using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Account
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
