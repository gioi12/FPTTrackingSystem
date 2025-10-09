using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class SemesterWeek
{
    public int Id { get; set; }

    public int? SemesterId { get; set; }

    public int? WeekNumber { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public bool? IsVacation { get; set; }

    public virtual Semester? Semester { get; set; }
}
