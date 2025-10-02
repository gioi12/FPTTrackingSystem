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

    public int? StatusId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<GroupAttachment> GroupAttachments { get; set; } = new List<GroupAttachment>();

    public virtual Major? Major { get; set; }

    public virtual ICollection<MilestoneAttachment> MilestoneAttachments { get; set; } = new List<MilestoneAttachment>();

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual ICollection<Report> ReportCreateByNavigations { get; set; } = new List<Report>();

    public virtual ICollection<Report> ReportStudents { get; set; } = new List<Report>();

    public virtual Status? Status { get; set; }

    public virtual ICollection<TaskAttachment> TaskAttachments { get; set; } = new List<TaskAttachment>();
    public virtual ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();

}
