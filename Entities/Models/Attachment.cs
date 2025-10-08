using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Attachment
{
    public int Id { get; set; }

    public string EntityName { get; set; } = null!;

    public int EntityId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime CreateAt { get; set; }

    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
