using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Deliverable
{
    public int Id { get; set; }

    public int MilestoneId { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public int SemesterId { get; set; }

    public int StatusId { get; set; }

    public string? Description { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

    public virtual Milestone Milestone { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
