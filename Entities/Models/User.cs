using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class User
{
    public int Id { get; set; }

    public string? RollNumber { get; set; }

    public int? AccountId { get; set; }

    public string? Fullname { get; set; }

    public DateOnly? Dob { get; set; }

    public bool? Gender { get; set; }

    public string? Mail { get; set; }

    public string? Phone { get; set; }

    public int? MajorId { get; set; }

    public string? CapstoneProject { get; set; }

    public string? Address { get; set; }

    public string? StatusId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Evaluation> EvaluationEvaluators { get; set; } = new List<Evaluation>();

    public virtual ICollection<Evaluation> EvaluationReceivers { get; set; } = new List<Evaluation>();

    public virtual ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual MajorCategory? Major { get; set; }

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual Status? Status { get; set; }

    public virtual ICollection<TaskUser> TaskUsers { get; set; } = new List<TaskUser>();
}
