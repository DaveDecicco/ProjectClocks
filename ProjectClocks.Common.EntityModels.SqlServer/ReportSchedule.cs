using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("ReportSchedules", Schema = "projectclocks")]
    public partial class ReportSchedule
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("group_id")]
        public int GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("cron_spec")]
        [StringLength(255)]
        public string CronSpec { get; set; } = null!;
        [Column("last")]
        public int? Last { get; set; }
        [Column("next")]
        public int? Next { get; set; }
        [Column("report_id")]
        public int? ReportId { get; set; }
        [Column("email")]
        [StringLength(100)]
        public string? Email { get; set; }
        [Column("cc")]
        [StringLength(100)]
        public string? Cc { get; set; }
        [Column("subject")]
        [StringLength(100)]
        public string? Subject { get; set; }
        [Column("comment")]
        public string? Comment { get; set; }
        [Column("report_condition")]
        [StringLength(255)]
        public string? ReportCondition { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
