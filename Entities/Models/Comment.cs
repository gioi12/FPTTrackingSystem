using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Comment
{
    public int Id { get; set; }

    public string Feedback { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public int UserId { get; set; }

    public int? TaskId { get; set; }

    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Task? Task { get; set; }

    public virtual User User { get; set; } = null!;
}
