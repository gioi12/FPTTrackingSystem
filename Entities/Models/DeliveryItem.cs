using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class DeliveryItem
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? DeliverableId { get; set; }

    public int? MilestoneItemId { get; set; }

    public virtual Deliverable? Deliverable { get; set; }

    public virtual MilestoneItem? MilestoneItem { get; set; }
}
