using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Couse
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
