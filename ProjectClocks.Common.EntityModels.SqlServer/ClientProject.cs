using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Keyless]
    [Table("ClientProjects", Schema = "projectclocks")]
    [Index("ClientId", Name = "client_idx")]
    [Index("ProjectId", Name = "project_idx")]
    public partial class ClientProject
    {
        [Column("client_id")]
        public int ClientId { get; set; }
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
    }
}
