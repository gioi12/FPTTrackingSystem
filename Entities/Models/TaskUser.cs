using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class TaskUser
{
    public int UserId { get; set; }

    public int TaskId { get; set; }

    public string Type { get; set; } = null!;

    public int Id { get; set; }

    public bool? IsCreated { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
