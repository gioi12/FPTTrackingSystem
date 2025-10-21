using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class SemesterVacation
{
    public int Id { get; set; }

    public int SemesterId { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public string? Description { get; set; }

    public virtual Semester Semester { get; set; } = null!;
}
