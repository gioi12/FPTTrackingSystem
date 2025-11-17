using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Group
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? SemesterId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string? Profession { get; set; }

    public int? MajorId { get; set; }

    public string? Description { get; set; }

    public string? VietnameseTitle { get; set; }

    public string? StatusId { get; set; }

    public int? MeetingId { get; set; }

    public DateTime? ExpireDate { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<DeliverableGroup> DeliverableGroups { get; set; } = new List<DeliverableGroup>();

    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

    public virtual ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();

    public virtual MajorCategory? Major { get; set; }

    public virtual Meeting? Meeting { get; set; }

    public virtual Semester? Semester { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<UserSlot> UserSlots { get; set; } = new List<UserSlot>();
}
