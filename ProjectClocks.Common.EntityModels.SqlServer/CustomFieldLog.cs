using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("CustomFieldLogs", Schema = "projectclocks")]
    [Index("LogId", Name = "log_idx")]
    public partial class CustomFieldLog
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("log_id")]
        public long LogId { get; set; }
        [Column("field_id")]
        public int FieldId { get; set; }
        [Column("option_id")]
        public int? OptionId { get; set; }
        [Column("value")]
        [StringLength(255)]
        public string? Value { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
