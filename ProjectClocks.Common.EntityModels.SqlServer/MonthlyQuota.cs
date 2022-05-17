using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("MonthlyQuotas", Schema = "projectclocks")]
    public partial class MonthlyQuota
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Key]
        [Column("year")]
        public int Year { get; set; }
        [Key]
        [Column("month")]
        public byte Month { get; set; }
        [Column("minutes")]
        public int? Minutes { get; set; }

        [ForeignKey("GroupId")]
        [InverseProperty("MonthlyQuota")]
        public virtual Group Group { get; set; } = null!;
    }
}
