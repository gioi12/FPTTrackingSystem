using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Milestone
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }

    public int MajorId { get; set; }

    public string? Description { get; set; }

    public string? Deadline { get; set; }

    public bool? IsActive { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();

    public virtual MajorCategory Major { get; set; } = null!;

    public virtual ICollection<MilestoneItem> MilestoneItems { get; set; } = new List<MilestoneItem>();
}
