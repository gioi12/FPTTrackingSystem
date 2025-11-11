using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class UserSlot
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? SlotId { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? GroupId { get; set; }

    public string? DayOfWeek { get; set; }

    public virtual Group? Group { get; set; }

    public virtual Slot? Slot { get; set; }

    public virtual User? User { get; set; }
}
