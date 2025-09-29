using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class GroupAttachment
{
    public int Id { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public int GroupId { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
