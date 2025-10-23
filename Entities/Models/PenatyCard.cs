using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class PenatyCard
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public int? EvaluationId { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Evaluation? Evaluation { get; set; }

    public virtual User? User { get; set; }
}
