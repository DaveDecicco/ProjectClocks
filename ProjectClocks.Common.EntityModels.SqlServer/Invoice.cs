using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Table("Invoices", Schema = "projectclocks")]
    [Index("GroupId", "Name", "Status", Name = "invoices$name_idx", IsUnique = true)]
    public partial class Invoice
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
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }
        [Column("client_id")]
        public int ClientId { get; set; }
        [Column("status")]
        public short? Status { get; set; }
    }
}
