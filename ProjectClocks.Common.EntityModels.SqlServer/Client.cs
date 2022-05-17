using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("Clients", Schema = "projectclocks")]
    [Index("GroupId", "Name", "Status", Name = "clients$client_name_idx", IsUnique = true)]
    public partial class Client
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
        [Column("address")]
        [StringLength(255)]
        public string? Address { get; set; }
        [Column("tax", TypeName = "numeric(6, 2)")]
        public decimal? Tax { get; set; }
        [Column("projects")]
        public string? Projects { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
