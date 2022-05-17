using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("TimeSheets", Schema = "projectclocks")]
    public partial class TimeSheet
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("client_id")]
        public int? ClientId { get; set; }
        [Column("project_id")]
        public int? ProjectId { get; set; }
        [Column("name")]
        [StringLength(80)]
        public string Name { get; set; } = null!;
        [Column("comment")]
        public string? Comment { get; set; }
        [Column("start_date", TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column("end_date", TypeName = "date")]
        public DateTime EndDate { get; set; }
        [Column("submit_status")]
        public short? SubmitStatus { get; set; }
        [Column("approve_status")]
        public short? ApproveStatus { get; set; }
        [Column("approve_comment")]
        public string? ApproveComment { get; set; }
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
