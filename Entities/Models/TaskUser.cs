using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class TaskUser
{
    public int UserId { get; set; }

    public int TaskId { get; set; }

    public bool? IsCreated { get; set; }

    public int Id { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
