using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Major
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<MajorCategory> MajorCategories { get; set; } = new List<MajorCategory>();
}
