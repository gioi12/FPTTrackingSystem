using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Report
{
    public int Id { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }

    public int? GroupId { get; set; }

    public int? StudentId { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual User? Student { get; set; }
}
