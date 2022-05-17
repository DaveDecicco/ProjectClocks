using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("FileAttachments", Schema = "projectclocks")]
    public partial class FileAttachment
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("group_id")]
        public long? GroupId { get; set; }
        [Column("org_id")]
        public long? OrgId { get; set; }
        [Column("remote_id")]
        public long? RemoteId { get; set; }
        [Column("file_key")]
        [StringLength(32)]
        public string? FileKey { get; set; }
        [Column("entity_type")]
        [StringLength(32)]
        public string? EntityType { get; set; }
        [Column("entity_id")]
        public long? EntityId { get; set; }
        [Column("file_name")]
        [StringLength(80)]
        public string FileName { get; set; } = null!;
        [Column("description")]
        [StringLength(255)]
        public string? Description { get; set; }
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
