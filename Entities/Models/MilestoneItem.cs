using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MilestoneItem
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? MilestoneId { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? CreateBy { get; set; }

    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

    public virtual Milestone? Milestone { get; set; }
}
