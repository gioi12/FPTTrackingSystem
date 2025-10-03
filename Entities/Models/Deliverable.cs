using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Deliverable
{
    public int Id { get; set; }

    public int MilestoneId { get; set; }

    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Milestone Milestone { get; set; } = null!;

    public virtual ICollection<MilestoneAttachment> MilestoneAttachments { get; set; } = new List<MilestoneAttachment>();
}
