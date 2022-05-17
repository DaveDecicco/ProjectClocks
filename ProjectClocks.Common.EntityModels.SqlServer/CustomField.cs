using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("CustomFields", Schema = "projectclocks")]
    public partial class CustomField
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("group_id")]
        public int GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("entity_type")]
        public short? EntityType { get; set; }
        [Column("type")]
        public short Type { get; set; }
        [Column("label")]
        [StringLength(32)]
        public string Label { get; set; } = null!;
        [Column("required")]
        public short? Required { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
