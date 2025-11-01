using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Evaluation
{
    public int Id { get; set; }

    public int ReceiverId { get; set; }

    public int EvaluatorId { get; set; }

    public string? Feedback { get; set; }

    public DateTime? CreateAt { get; set; }

    public string? Type { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int GroupId { get; set; }

    public int? DeliverableId { get; set; }

    public virtual Deliverable? Deliverable { get; set; }

    public virtual User Evaluator { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual User Receiver { get; set; } = null!;
}
