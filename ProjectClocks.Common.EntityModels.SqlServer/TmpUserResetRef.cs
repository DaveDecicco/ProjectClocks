using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Keyless]
    [Table("TmpUserResetRefs", Schema = "projectclocks")]
    public partial class TmpUserResetRef
    {
        [Column("created")]
        [Precision(0)]
        public DateTime? Created { get; set; }
        [Column("ref")]
        [StringLength(32)]
        public string Ref { get; set; } = null!;
        [Column("user_id")]
        public int UserId { get; set; }
    }
}
