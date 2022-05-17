using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Keyless]
    [Table("ProjectTemplates", Schema = "projectclocks")]
    [Index("ProjectId", Name = "project_idx")]
    [Index("TemplateId", Name = "template_idx")]
    public partial class ProjectTemplate
    {
        [Column("project_id")]
        public long ProjectId { get; set; }
        [Column("template_id")]
        public long TemplateId { get; set; }
        [Column("group_id")]
        public long GroupId { get; set; }
        [Column("org_id")]
        public long OrgId { get; set; }
    }
}
