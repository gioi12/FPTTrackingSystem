using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class TaskAttachment
{
    public int Id { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public int TaskId { get; set; }

    public int StatusId { get; set; }

    public int UserId { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
