using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Task
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public int StatusId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
