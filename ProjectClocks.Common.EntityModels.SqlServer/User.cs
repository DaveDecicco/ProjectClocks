using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("Users", Schema = "projectclocks")]
    [Index("Login", "Status", Name = "users$login_idx", IsUnique = true)]
    public partial class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("login")]
        [StringLength(50)]
        public string Login { get; set; } = null!;
        [Column("password")]
        [StringLength(50)]
        public string? Password { get; set; }
        [Column("name")]
        [StringLength(100)]
        public string? Name { get; set; }
        [Column("group_id")]
        public int GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("role_id")]
        public int? RoleId { get; set; }
        [Column("client_id")]
        public int? ClientId { get; set; }
        [Column("rate", TypeName = "numeric(6, 2)")]
        public decimal Rate { get; set; }
        [Column("quota_percent", TypeName = "numeric(6, 2)")]
        public decimal QuotaPercent { get; set; }
        [Column("email")]
        [StringLength(100)]
        public string? Email { get; set; }
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
        [Column("accessed")]
        [Precision(0)]
        public DateTime? Accessed { get; set; }
        [Column("accessed_ip")]
        [StringLength(45)]
        public string? AccessedIp { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
