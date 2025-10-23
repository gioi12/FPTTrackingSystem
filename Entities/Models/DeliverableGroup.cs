using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class DeliverableGroup
{
    public int Id { get; set; }

    public int? DeliverableId { get; set; }

    public int? GroupId { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public virtual Deliverable? Deliverable { get; set; }

    public virtual Group? Group { get; set; }
}
