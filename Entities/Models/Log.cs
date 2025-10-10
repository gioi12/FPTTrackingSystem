using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Log
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public int EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? Description { get; set; }

    public int UserId { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual User User { get; set; } = null!;


}
