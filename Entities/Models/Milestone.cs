using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Milestone
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }

    public int? SemesterId { get; set; }

    public int MajorId { get; set; }

    public int GroupId { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public string? Description { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Major Major { get; set; } = null!;

    public virtual ICollection<MilestoneAttachment> MilestoneAttachments { get; set; } = new List<MilestoneAttachment>();

    public virtual Semester? Semester { get; set; }
}
