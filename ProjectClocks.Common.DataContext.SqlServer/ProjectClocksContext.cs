using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    public partial class ProjectClocksContext : DbContext
    {
        public ProjectClocksContext()
        {
        }

        public ProjectClocksContext(DbContextOptions<ProjectClocksContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; } = null!;
        public virtual DbSet<ClientProject> ClientProjects { get; set; } = null!;
        public virtual DbSet<CustomField> CustomFields { get; set; } = null!;
        public virtual DbSet<CustomFieldLog> CustomFieldLogs { get; set; } = null!;
        public virtual DbSet<CustomFieldOption> CustomFieldOptions { get; set; } = null!;
        public virtual DbSet<EntityCustomField> EntityCustomFields { get; set; } = null!;
        public virtual DbSet<ExpenseItem> ExpenseItems { get; set; } = null!;
        public virtual DbSet<FileAttachment> FileAttachments { get; set; } = null!;
        public virtual DbSet<Group> Groups { get; set; } = null!;
        public virtual DbSet<GroupTemplate> GroupTemplates { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<MonthlyQuota> MonthlyQuotas { get; set; } = null!;
        public virtual DbSet<PredefinedExpense> PredefinedExpenses { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<ProjectTask> ProjectTasks { get; set; } = null!;
        public virtual DbSet<ProjectTemplate> ProjectTemplates { get; set; } = null!;
        public virtual DbSet<ReportConfiguration> ReportConfigurations { get; set; } = null!;
        public virtual DbSet<ReportSchedule> ReportSchedules { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<TimeSheet> TimeSheets { get; set; } = null!;
        public virtual DbSet<TmpUserResetRef> TmpUserResetRefs { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserConfiguration> UserConfigurations { get; set; } = null!;
        public virtual DbSet<UserProject> UserProjects { get; set; } = null!;
        public virtual DbSet<UserTimeEntry> UserTimeEntries { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ProjectClocks;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.Tax).HasDefaultValueSql("((0.00))");
            });

            modelBuilder.Entity<ClientProject>(entity =>
            {
                entity.HasIndex(e => new { e.ClientId, e.ProjectId }, "clientprojects$client_project_idx")
                    .IsUnique()
                    .IsClustered();
            });

            modelBuilder.Entity<CustomField>(entity =>
            {
                entity.Property(e => e.EntityType).HasDefaultValueSql("((1))");

                entity.Property(e => e.Label).HasDefaultValueSql("(N'')");

                entity.Property(e => e.Required).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<CustomFieldLog>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<CustomFieldOption>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.Value).HasDefaultValueSql("(N'')");
            });

            modelBuilder.Entity<EntityCustomField>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ExpenseItem>(entity =>
            {
                entity.Property(e => e.Approved).HasDefaultValueSql("((0))");

                entity.Property(e => e.Cost).HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Paid).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<FileAttachment>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.CustomLogo).HasDefaultValueSql("((0))");

                entity.Property(e => e.DateFormat).HasDefaultValueSql("(N'%Y-%m-%d')");

                entity.Property(e => e.DecimalMark)
                    .HasDefaultValueSql("(N'.')")
                    .IsFixedLength();

                entity.Property(e => e.Lang).HasDefaultValueSql("(N'en')");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.TimeFormat).HasDefaultValueSql("(N'%H:%M')");

                entity.Property(e => e.TrackingMode).HasDefaultValueSql("((1))");

                entity.Property(e => e.WorkdayMinutes).HasDefaultValueSql("((480))");
            });

            modelBuilder.Entity<GroupTemplate>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<MonthlyQuota>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.Year, e.Month })
                    .HasName("PK_monthlyquotas_group_id");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.MonthlyQuota)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MonthlyQuotas_Groups");
            });

            modelBuilder.Entity<PredefinedExpense>(entity =>
            {
                entity.Property(e => e.Cost).HasDefaultValueSql("((0.00))");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.HasIndex(e => new { e.ProjectId, e.TaskId }, "projecttasks$project_task_idx")
                    .IsUnique()
                    .IsClustered();
            });

            modelBuilder.Entity<ProjectTemplate>(entity =>
            {
                entity.HasIndex(e => new { e.ProjectId, e.TemplateId }, "projecttemplates$project_template_idx")
                    .IsUnique()
                    .IsClustered();
            });

            modelBuilder.Entity<ReportConfiguration>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ReportSchedule>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Rank).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TimeSheet>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<TmpUserResetRef>(entity =>
            {
                entity.Property(e => e.Ref)
                    .HasDefaultValueSql("(N'')")
                    .IsFixedLength();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.QuotaPercent).HasDefaultValueSql("((100.00))");

                entity.Property(e => e.Rate).HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserConfiguration>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.ParamName }, "userconfigurations$param_idx")
                    .IsUnique()
                    .IsClustered();
            });

            modelBuilder.Entity<UserProject>(entity =>
            {
                entity.Property(e => e.Rate).HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserTimeEntry>(entity =>
            {
                entity.Property(e => e.Approved).HasDefaultValueSql("((0))");

                entity.Property(e => e.Billable).HasDefaultValueSql("((0))");

                entity.Property(e => e.Paid).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
