using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Group
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? SemesterId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string? Profession { get; set; }

    public int? MajorId { get; set; }

    public string? Description { get; set; }

    public string? VietnameseTitle { get; set; }

    public int? CourseId { get; set; }

    public int? StatusId { get; set; }

    public virtual Couse? Course { get; set; }

    public virtual Major? Major { get; set; }

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual Semester? Semester { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
