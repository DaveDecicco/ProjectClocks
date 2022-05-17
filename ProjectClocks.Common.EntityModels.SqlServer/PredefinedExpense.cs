using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("PredefinedExpenses", Schema = "projectclocks")]
    public partial class PredefinedExpense
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("group_id")]
        public int GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; } = null!;
        [Column("cost", TypeName = "decimal(10, 2)")]
        public decimal? Cost { get; set; }
    }
}
