using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Slot
{
    public int Id { get; set; }

    public string? NameSlot { get; set; }

    public TimeOnly? StartAt { get; set; }

    public TimeOnly? EndAt { get; set; }

    public int? CampusId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public virtual ICollection<UserSlot> UserSlots { get; set; } = new List<UserSlot>();
}
