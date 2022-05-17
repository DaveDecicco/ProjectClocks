using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("Groups", Schema = "projectclocks")]
    public partial class Group
    {
        public Group()
        {
            MonthlyQuota = new HashSet<MonthlyQuota>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("parent_id")]
        public int? ParentId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("group_key")]
        [StringLength(32)]
        public string? GroupKey { get; set; }
        [Column("name")]
        [StringLength(80)]
        public string? Name { get; set; }
        [Column("description")]
        [StringLength(255)]
        public string? Description { get; set; }
        [Column("currency")]
        [StringLength(7)]
        public string? Currency { get; set; }
        [Column("decimal_mark")]
        [StringLength(1)]
        public string DecimalMark { get; set; } = null!;
        [Column("lang")]
        [StringLength(10)]
        public string Lang { get; set; } = null!;
        [Column("date_format")]
        [StringLength(20)]
        public string DateFormat { get; set; } = null!;
        [Column("time_format")]
        [StringLength(20)]
        public string TimeFormat { get; set; } = null!;
        [Column("week_start")]
        public short WeekStart { get; set; }
        [Column("tracking_mode")]
        public short TrackingMode { get; set; }
        [Column("project_required")]
        public short ProjectRequired { get; set; }
        [Column("record_type")]
        public short RecordType { get; set; }
        [Column("bcc_email")]
        [StringLength(100)]
        public string? BccEmail { get; set; }
        [Column("allow_ip")]
        [StringLength(255)]
        public string? AllowIp { get; set; }
        [Column("password_complexity")]
        [StringLength(64)]
        public string? PasswordComplexity { get; set; }
        [Column("plugins")]
        [StringLength(255)]
        public string? Plugins { get; set; }
        [Column("lock_spec")]
        [StringLength(255)]
        public string? LockSpec { get; set; }
        [Column("holidays")]
        public string? Holidays { get; set; }
        [Column("workday_minutes")]
        public short? WorkdayMinutes { get; set; }
        [Column("custom_logo")]
        public short? CustomLogo { get; set; }
        [Column("config")]
        public string? Config { get; set; }
        [Column("custom_css")]
        public string? CustomCss { get; set; }
        [Column("created")]
        [Precision(0)]
        public DateTime? Created { get; set; }
        [Column("created_ip")]
        [StringLength(45)]
        public string? CreatedIp { get; set; }
        [Column("created_by")]
        public int? CreatedBy { get; set; }
        [Column("modified")]
        [Precision(0)]
        public DateTime? Modified { get; set; }
        [Column("modified_ip")]
        [StringLength(45)]
        public string? ModifiedIp { get; set; }
        [Column("modified_by")]
        public int? ModifiedBy { get; set; }
        [Column("entities_modified")]
        [Precision(0)]
        public DateTime? EntitiesModified { get; set; }
        [Column("status")]
        public short? Status { get; set; }

        [InverseProperty("Group")]
        public virtual ICollection<MonthlyQuota> MonthlyQuota { get; set; }
    }
}
