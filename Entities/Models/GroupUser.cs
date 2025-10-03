using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class GroupUser
{
    public int UserId { get; set; }

    public int GroupId { get; set; }

    public bool IsActive { get; set; }

    public int Id { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
