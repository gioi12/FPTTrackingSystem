using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Aisetting
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? SecretKey { get; set; }

    public bool? IsActive { get; set; }
}
