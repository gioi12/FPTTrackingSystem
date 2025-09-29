using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class TaskFeedback
{
    public int Id { get; set; }

    public DateTime CreateAt { get; set; }

    public string? Message { get; set; }

    public int? TaskId { get; set; }

    public virtual Task? Task { get; set; }
}
