using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Deliverable
{
    public int Id { get; set; }

    public int MilestoneId { get; set; }

    public int SemesterId { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = null!;

    public string? Deadline { get; set; }

    public bool? IsActive { get; set; }

    public int? MajorId { get; set; }

    public virtual ICollection<DeliverableGroup> DeliverableGroups { get; set; } = new List<DeliverableGroup>();

    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

    public virtual MajorCategory? Major { get; set; }

    public virtual Milestone Milestone { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
