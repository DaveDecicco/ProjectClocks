using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("Roles", Schema = "projectclocks")]
    [Index("GroupId", "Rank", "Status", Name = "roles$role_idx", IsUnique = true)]
    public partial class Role
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
        public string? Name { get; set; }
        [Column("description")]
        [StringLength(255)]
        public string? Description { get; set; }
        [Column("rank")]
        public int? Rank { get; set; }
        [Column("rights")]
        public string? Rights { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
