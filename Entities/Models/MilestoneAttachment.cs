using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MilestoneAttachment
{
    public int Id { get; set; }

    public string? AttachmentPath { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? DeliverableId { get; set; }

    public int? CreateBy { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual Deliverable? Deliverable { get; set; }
}
