using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("Projects", Schema = "projectclocks")]
    [Index("GroupId", "Name", "Status", Name = "projects$project_idx", IsUnique = true)]
    public partial class Project
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("group_id")]
        public int GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("name")]
        [StringLength(80)]
        public string Name { get; set; } = null!;
        [Column("description")]
        [StringLength(255)]
        public string? Description { get; set; }
        [Column("tasks")]
        public string? Tasks { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
