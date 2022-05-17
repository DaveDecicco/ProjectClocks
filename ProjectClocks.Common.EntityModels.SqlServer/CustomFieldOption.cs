using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("CustomFieldOptions", Schema = "projectclocks")]
    public partial class CustomFieldOption
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("field_id")]
        public int FieldId { get; set; }
        [Column("value")]
        [StringLength(32)]
        public string Value { get; set; } = null!;
        [Column("status")]
        public short? Status { get; set; }
    }
}
