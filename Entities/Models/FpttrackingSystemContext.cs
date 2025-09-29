using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models;

public partial class FpttrackingSystemContext : DbContext
{
    public FpttrackingSystemContext()
    {
    }

    public FpttrackingSystemContext(DbContextOptions<FpttrackingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupAttachment> GroupAttachments { get; set; }

    public virtual DbSet<GroupUser> GroupUsers { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<Milestone> Milestones { get; set; }

    public virtual DbSet<MilestoneAttachment> MilestoneAttachments { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskAttachment> TaskAttachments { get; set; }

    public virtual DbSet<TaskFeedback> TaskFeedbacks { get; set; }

    public virtual DbSet<TaskUser> TaskUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server =160.30.21.113; database = FPTTrackingSystem;uid=sa;pwd=123;TrustServerCertificate=True");

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

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Profession)
                .HasMaxLength(50)
                .HasColumnName("profession");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.VietnameseTitle)
                .HasMaxLength(200)
                .HasColumnName("vietnamese_title");

            entity.HasOne(d => d.Major).WithMany(p => p.Groups)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_Group_Major");

            entity.HasOne(d => d.Semester).WithMany(p => p.Groups)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_Group_Semester");

            entity.HasOne(d => d.Status).WithMany(p => p.Groups)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Group_Status");
        });

        modelBuilder.Entity<GroupAttachment>(entity =>
        {
            entity.ToTable("Group_Attachment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttachmentPath).HasColumnName("attachment_path");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.GroupAttachments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_Attachment_User");
        });

        modelBuilder.Entity<GroupUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Group_User");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany()
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_User_Group");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Group_User_User");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.ToTable("Major");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.ToTable("Milestone");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SemesterId).HasColumnName("semester_id");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Milestone_User");

            entity.HasOne(d => d.Group).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Milestone_Group");

            entity.HasOne(d => d.Major).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.MajorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Milestone_Major");

            entity.HasOne(d => d.Semester).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_Milestone_Semester");
        });

        modelBuilder.Entity<MilestoneAttachment>(entity =>
        {
            entity.ToTable("Milestone_Attachment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttachmentPath).HasColumnName("attachment_path");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.MilestoneAttachments)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Milestone_Attachment_User");

            entity.HasOne(d => d.Milestone).WithMany(p => p.MilestoneAttachments)
                .HasForeignKey(d => d.MilestoneId)
                .HasConstraintName("FK_Milestone_Attachment_Milestone");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("Report");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.ReportCreateByNavigations)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK_Report_User");

            entity.HasOne(d => d.Student).WithMany(p => p.ReportStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Report_User1");
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

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("name");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("Task");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");

            entity.HasOne(d => d.Group).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_Group");
        });

        modelBuilder.Entity<TaskAttachment>(entity =>
        {
            entity.ToTable("Task_Attachment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttachmentPath).HasColumnName("attachment_path");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Status).WithMany(p => p.TaskAttachments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_Attachment_Status");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAttachments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_Attachment_Task");

            entity.HasOne(d => d.User).WithMany(p => p.TaskAttachments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_Attachment_User");
        });

        modelBuilder.Entity<TaskFeedback>(entity =>
        {
            entity.ToTable("Task_Feedback");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.TaskId).HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskFeedbacks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Task_Feedback_Task");
        });

        modelBuilder.Entity<TaskUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Task_User");

            entity.Property(e => e.IsCreated).HasColumnName("is_created");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany()
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_User_Task");

            entity.HasOne(d => d.User).WithMany()
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
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Account).WithMany(p => p.Users)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_User_Account");

            entity.HasOne(d => d.Major).WithMany(p => p.Users)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("FK_User_Major");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_User_Status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
