using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("EntityCustomFields", Schema = "projectclocks")]
    [Index("EntityType", "EntityId", "FieldId", Name = "entitycustomfields$entity_idx", IsUnique = true)]
    public partial class EntityCustomField
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("group_id")]
        public long GroupId { get; set; }
        [Column("org_id")]
        public long OrgId { get; set; }
        [Column("entity_type")]
        public short EntityType { get; set; }
        [Column("entity_id")]
        public long EntityId { get; set; }
        [Column("field_id")]
        public long FieldId { get; set; }
        [Column("option_id")]
        public long? OptionId { get; set; }
        [Column("value")]
        [StringLength(255)]
        public string? Value { get; set; }
        [Column("created")]
        [Precision(0)]
        public DateTime? Created { get; set; }
        [Column("created_ip")]
        [StringLength(45)]
        public string? CreatedIp { get; set; }
        [Column("created_by")]
        public long? CreatedBy { get; set; }
        [Column("modified")]
        [Precision(0)]
        public DateTime? Modified { get; set; }
        [Column("modified_ip")]
        [StringLength(45)]
        public string? ModifiedIp { get; set; }
        [Column("modified_by")]
        public long? ModifiedBy { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
