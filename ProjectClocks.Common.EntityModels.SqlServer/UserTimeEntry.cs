using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("UserTimeEntries", Schema = "projectclocks")]
    [Index("ClientId", Name = "client_idx")]
    [Index("Date", Name = "date_idx")]
    [Index("GroupId", Name = "group_idx")]
    [Index("InvoiceId", Name = "invoice_idx")]
    [Index("ProjectId", Name = "project_idx")]
    [Index("TaskId", Name = "task_idx")]
    [Index("TimesheetId", Name = "timesheet_idx")]
    [Index("UserId", Name = "user_idx")]
    public partial class UserTimeEntry
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }
        [Column("start")]
        public TimeSpan? Start { get; set; }
        [Column("duration")]
        public TimeSpan? Duration { get; set; }
        [Column("client_id")]
        public int? ClientId { get; set; }
        [Column("project_id")]
        public int? ProjectId { get; set; }
        [Column("task_id")]
        public int? TaskId { get; set; }
        [Column("timesheet_id")]
        public int? TimesheetId { get; set; }
        [Column("invoice_id")]
        public int? InvoiceId { get; set; }
        [Column("comment")]
        public string? Comment { get; set; }
        [Column("billable")]
        public short? Billable { get; set; }
        [Column("approved")]
        public short? Approved { get; set; }
        [Column("paid")]
        public short? Paid { get; set; }
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
        [Column("status")]
        public short? Status { get; set; }
    }
}
