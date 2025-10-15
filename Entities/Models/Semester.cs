using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Semester
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public string? Description { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<SemesterWeek> SemesterWeeks { get; set; } = new List<SemesterWeek>();
}
