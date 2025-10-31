using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models;

public partial class FpttrackingSystemContext : DbContext
{
    public FpttrackingSystemContext(DbContextOptions<FpttrackingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Deliverable> Deliverables { get; set; }

    public virtual DbSet<DeliverableGroup> DeliverableGroups { get; set; }

    public virtual DbSet<DeliveryItem> DeliveryItems { get; set; }

    public virtual DbSet<Evaluation> Evaluations { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupUser> GroupUsers { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<MajorCategory> MajorCategories { get; set; }

    public virtual DbSet<Meeting> Meetings { get; set; }

    public virtual DbSet<MeetingMinute> MeetingMinutes { get; set; }

    public virtual DbSet<MeetingScheduleDate> MeetingScheduleDates { get; set; }

    public virtual DbSet<Milestone> Milestones { get; set; }

    public virtual DbSet<MilestoneItem> MilestoneItems { get; set; }

    public virtual DbSet<PenatyCard> PenatyCards { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<SemesterVacation> SemesterVacations { get; set; }

    public virtual DbSet<SemesterWeek> SemesterWeeks { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskUser> TaskUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("Attachment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityName)
                .HasMaxLength(50)
                .HasColumnName("entity_name");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsDownload).HasColumnName("is_download");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachment_Group");

            entity.HasOne(d => d.User).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachment_User");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany(p => p.Comments)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_Group");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Comment_Task");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<Deliverable>(entity =>
        {
            entity.ToTable("Deliverable");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Deadline)
                .HasMaxLength(50)
                .HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");

            entity.HasOne(d => d.Major).WithMany(p => p.Deliverables)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_Deliverable_Major_Category");

            entity.HasOne(d => d.Milestone).WithMany(p => p.Deliverables)
                .HasForeignKey(d => d.MilestoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deliverable_Milestone");

            entity.HasOne(d => d.Semester).WithMany(p => p.Deliverables)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deliverable_Semester");
        });

        modelBuilder.Entity<DeliverableGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Deliverable_Group_1");

            entity.ToTable("Deliverable_Group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeliverableId).HasColumnName("deliverable_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Note)
                .HasMaxLength(200)
                .HasColumnName("note");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Deliverable).WithMany(p => p.DeliverableGroups)
                .HasForeignKey(d => d.DeliverableId)
                .HasConstraintName("FK_Deliverable_Group_Deliverable");

            entity.HasOne(d => d.Group).WithMany(p => p.DeliverableGroups)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Deliverable_Group_Group");
        });

        modelBuilder.Entity<DeliveryItem>(entity =>
        {
            entity.ToTable("Delivery_item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeliverableId).HasColumnName("deliverable_id");
            entity.Property(e => e.Description)
                .HasMaxLength(510)
                .HasColumnName("description");
            entity.Property(e => e.MilestoneItemId).HasColumnName("milestone_item_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Deliverable).WithMany(p => p.DeliveryItems)
                .HasForeignKey(d => d.DeliverableId)
                .HasConstraintName("FK_Delivery_item_Deliverable");

            entity.HasOne(d => d.MilestoneItem).WithMany(p => p.DeliveryItems)
                .HasForeignKey(d => d.MilestoneItemId)
                .HasConstraintName("FK_Delivery_item_Milestone_Item");
        });

        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.ToTable("Evaluation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.DeliverableId).HasColumnName("deliverable_id");
            entity.Property(e => e.EvaluatorId).HasColumnName("evaluator_id");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Deliverable).WithMany(p => p.Evaluations)
                .HasForeignKey(d => d.DeliverableId)
                .HasConstraintName("FK_Evaluation_Deliverable");

            entity.HasOne(d => d.Evaluator).WithMany(p => p.EvaluationEvaluators)
                .HasForeignKey(d => d.EvaluatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evaluation_User1");

            entity.HasOne(d => d.Group).WithMany(p => p.Evaluations)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evaluation_Group");

            entity.HasOne(d => d.Receiver).WithMany(p => p.EvaluationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evaluation_User");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Profession)
                .HasMaxLength(50)
                .HasColumnName("profession");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.StatusId)
                .HasMaxLength(50)
                .HasColumnName("status_id");
            entity.Property(e => e.VietnameseTitle)
                .HasMaxLength(200)
                .HasColumnName("vietnamese_title");

            entity.HasOne(d => d.Major).WithMany(p => p.Groups)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_Group_Major_Category");

            entity.HasOne(d => d.Meeting).WithMany(p => p.Groups)
                .HasForeignKey(d => d.MeetingId)
                .HasConstraintName("FK_Group_Meeting");

            entity.HasOne(d => d.Semester).WithMany(p => p.Groups)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_Group_Semester");

            entity.HasOne(d => d.Status).WithMany(p => p.Groups)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Group_Status");
        });

        modelBuilder.Entity<GroupUser>(entity =>
        {
            entity.ToTable("Group_User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(50)
                .HasColumnName("day_of_week");
            entity.Property(e => e.FreeTime)
                .HasMaxLength(300)
                .HasColumnName("free_time");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupUsers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_User_Group");

            entity.HasOne(d => d.User).WithMany(p => p.GroupUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_User_User");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityName)
                .HasMaxLength(50)
                .HasColumnName("entity_name");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Log_User");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.ToTable("Major");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<MajorCategory>(entity =>
        {
            entity.ToTable("Major_Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.MajorId).HasColumnName("Major_Id");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Major).WithMany(p => p.MajorCategories)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_Major_Category_Major");
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.ToTable("Meeting");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(50)
                .HasColumnName("day_of_week");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MeetingLink)
                .HasMaxLength(200)
                .HasColumnName("meeting_link");
            entity.Property(e => e.Time)
                .HasMaxLength(50)
                .HasColumnName("time");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Meetings)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Meeting_User");
        });

        modelBuilder.Entity<MeetingMinute>(entity =>
        {
            entity.ToTable("Meeting_Minute");

            entity.HasIndex(e => e.MeetingScheduleDateId, "UQ_Meeting_Minute").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attendance)
                .HasMaxLength(300)
                .HasColumnName("attendance");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.Issue).HasColumnName("issue");
            entity.Property(e => e.MeetingContent).HasColumnName("meeting_content");
            entity.Property(e => e.MeetingScheduleDateId).HasColumnName("meeting_schedule_date_id");
            entity.Property(e => e.Other).HasColumnName("other");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.MeetingMinutes)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Meeting_Minute_User1");

            entity.HasOne(d => d.MeetingScheduleDate).WithOne(p => p.MeetingMinute)
                .HasForeignKey<MeetingMinute>(d => d.MeetingScheduleDateId)
                .HasConstraintName("FK_Meeting_Minute_Meeting_Schedule_Date");
        });

        modelBuilder.Entity<MeetingScheduleDate>(entity =>
        {
            entity.ToTable("Meeting_Schedule_Date");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsMeeting).HasColumnName("is_Meeting");
            entity.Property(e => e.MeetingDate)
                .HasColumnType("datetime")
                .HasColumnName("meeting_date");
            entity.Property(e => e.MeetingId).HasColumnName("meeting_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingScheduleDates)
                .HasForeignKey(d => d.MeetingId)
                .HasConstraintName("FK_Meeting_Schedule_Date_Meeting");
        });

        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.ToTable("Milestone");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.Deadline)
                .HasMaxLength(50)
                .HasColumnName("deadline");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Milestone_User");

            entity.HasOne(d => d.Major).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.MajorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Milestone_Major_Category");
        });

        modelBuilder.Entity<MilestoneItem>(entity =>
        {
            entity.ToTable("Milestone_Item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Milestone).WithMany(p => p.MilestoneItems)
                .HasForeignKey(d => d.MilestoneId)
                .HasConstraintName("FK_Milestone_Item_Milestone");
        });

        modelBuilder.Entity<PenatyCard>(entity =>
        {
            entity.ToTable("Penaty_Card");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.EvaluationId).HasColumnName("evaluation_id");
            entity.Property(e => e.EvaluatorId).HasColumnName("evaluator_id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Evaluation).WithMany(p => p.PenatyCards)
                .HasForeignKey(d => d.EvaluationId)
                .HasConstraintName("FK_Penaty_Card_Evaluation");

            entity.HasOne(d => d.User).WithMany(p => p.PenatyCards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Penaty_Card_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Detail).HasColumnName("detail");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.ToTable("Semester");

            entity.HasIndex(e => e.IsActive, "UX_Semester_IsActive_OnlyOneTrue")
                .IsUnique()
                .HasFilter("([is_active]=(1))");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");
        });

        modelBuilder.Entity<SemesterVacation>(entity =>
        {
            entity.ToTable("Semester_Vacation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");

            entity.HasOne(d => d.Semester).WithMany(p => p.SemesterVacations)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Semester_Vacation_Semester");
        });

        modelBuilder.Entity<SemesterWeek>(entity =>
        {
            entity.ToTable("Semester_Week");

            entity.HasIndex(e => new { e.SemesterId, e.WeekNumber }, "UQ_Semester_Week");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.EndAtLunar)
                .HasColumnType("datetime")
                .HasColumnName("end_at_lunar");
            entity.Property(e => e.IsVacation).HasColumnName("is_vacation");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");
            entity.Property(e => e.StartAtLunar)
                .HasColumnType("datetime")
                .HasColumnName("start_at_lunar");
            entity.Property(e => e.WeekLearn).HasColumnName("week_learn");
            entity.Property(e => e.WeekNumber).HasColumnName("week_number");

            entity.HasOne(d => d.Semester).WithMany(p => p.SemesterWeeks)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_Semester_Week_Semester");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("Task");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_At");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.DeliverableId).HasColumnName("deliverable_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MeetingScheduleDateId).HasColumnName("meeting_schedule_date_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Priority)
                .HasMaxLength(50)
                .HasColumnName("priority");
            entity.Property(e => e.Process)
                .HasMaxLength(50)
                .HasColumnName("process");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.Deliverable).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.DeliverableId)
                .HasConstraintName("FK_Task_Deliverable");

            entity.HasOne(d => d.Group).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_Group");

            entity.HasOne(d => d.MeetingScheduleDate).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.MeetingScheduleDateId)
                .HasConstraintName("FK_Task_Meeting_Schedule_Date");

            entity.HasMany(d => d.TaskReferences).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskDependence",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskReferenceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Task_Dependence_Task1"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Task_Dependence_Task"),
                    j =>
                    {
                        j.HasKey("TaskId", "TaskReferenceId");
                        j.ToTable("Task_Dependence");
                        j.IndexerProperty<int>("TaskId").HasColumnName("task_id");
                        j.IndexerProperty<int>("TaskReferenceId").HasColumnName("task_reference_id");
                    });

            entity.HasMany(d => d.Tasks).WithMany(p => p.TaskReferences)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskDependence",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Task_Dependence_Task"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskReferenceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Task_Dependence_Task1"),
                    j =>
                    {
                        j.HasKey("TaskId", "TaskReferenceId");
                        j.ToTable("Task_Dependence");
                        j.IndexerProperty<int>("TaskId").HasColumnName("task_id");
                        j.IndexerProperty<int>("TaskReferenceId").HasColumnName("task_reference_id");
                    });
        });

        modelBuilder.Entity<TaskUser>(entity =>
        {
            entity.ToTable("Task_User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsCreated).HasColumnName("is_created");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskUsers)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_User_Task");

            entity.HasOne(d => d.User).WithMany(p => p.TaskUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_User_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.CapstoneProject)
                .HasMaxLength(200)
                .HasColumnName("capstone_project");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Mail)
                .HasMaxLength(100)
                .HasColumnName("mail");
            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");
            entity.Property(e => e.RollNumber)
                .HasMaxLength(50)
                .HasColumnName("roll_number");
            entity.Property(e => e.StatusId)
                .HasMaxLength(50)
                .HasColumnName("status_id");

            entity.HasOne(d => d.Account).WithMany(p => p.Users)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_User_Account");

            entity.HasOne(d => d.Major).WithMany(p => p.Users)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_User_Major_Category");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_User_Status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
