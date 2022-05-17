using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("ReportConfigurations", Schema = "projectclocks")]
    public partial class ReportConfiguration
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; } = null!;
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("report_spec")]
        public string? ReportSpec { get; set; }
        [Column("client_id")]
        public int? ClientId { get; set; }
        [Column("project_id")]
        public int? ProjectId { get; set; }
        [Column("task_id")]
        public int? TaskId { get; set; }
        [Column("billable")]
        public short? Billable { get; set; }
        [Column("approved")]
        public short? Approved { get; set; }
        [Column("invoice")]
        public short? Invoice { get; set; }
        [Column("timesheet")]
        public short? Timesheet { get; set; }
        [Column("paid_status")]
        public short? PaidStatus { get; set; }
        [Column("users")]
        public string? Users { get; set; }
        [Column("period")]
        public short? Period { get; set; }
        [Column("period_start", TypeName = "date")]
        public DateTime? PeriodStart { get; set; }
        [Column("period_end", TypeName = "date")]
        public DateTime? PeriodEnd { get; set; }
        [Column("show_client")]
        public short ShowClient { get; set; }
        [Column("show_invoice")]
        public short ShowInvoice { get; set; }
        [Column("show_paid")]
        public short ShowPaid { get; set; }
        [Column("show_ip")]
        public short ShowIp { get; set; }
        [Column("show_project")]
        public short ShowProject { get; set; }
        [Column("show_timesheet")]
        public short ShowTimesheet { get; set; }
        [Column("show_start")]
        public short ShowStart { get; set; }
        [Column("show_duration")]
        public short ShowDuration { get; set; }
        [Column("show_cost")]
        public short ShowCost { get; set; }
        [Column("show_task")]
        public short ShowTask { get; set; }
        [Column("show_end")]
        public short ShowEnd { get; set; }
        [Column("show_note")]
        public short ShowNote { get; set; }
        [Column("show_approved")]
        public short ShowApproved { get; set; }
        [Column("show_work_units")]
        public short ShowWorkUnits { get; set; }
        [Column("show_totals_only")]
        public short ShowTotalsOnly { get; set; }
        [Column("group_by1")]
        [StringLength(20)]
        public string? GroupBy1 { get; set; }
        [Column("group_by2")]
        [StringLength(20)]
        public string? GroupBy2 { get; set; }
        [Column("group_by3")]
        [StringLength(20)]
        public string? GroupBy3 { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
