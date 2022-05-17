using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("UserProjects", Schema = "projectclocks")]
    [Index("UserId", "ProjectId", Name = "userprojects$bind_idx", IsUnique = true)]
    public partial class UserProject
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("rate", TypeName = "numeric(6, 2)")]
        public decimal? Rate { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
