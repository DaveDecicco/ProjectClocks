using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Keyless]
    [Table("UserConfigurations", Schema = "projectclocks")]
    public partial class UserConfiguration
    {
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
        [Column("param_name")]
        [StringLength(32)]
        public string ParamName { get; set; } = null!;
        [Column("param_value")]
        [StringLength(80)]
        public string? ParamValue { get; set; }
    }
}
