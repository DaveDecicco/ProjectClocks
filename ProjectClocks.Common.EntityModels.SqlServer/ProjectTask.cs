using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectClocks.Common.EntityModels.SqlServer
{
    [Keyless]
    [Table("ProjectTasks", Schema = "projectclocks")]
    [Index("ProjectId", Name = "project_idx")]
    [Index("TaskId", Name = "task_idx")]
    public partial class ProjectTask
    {
        [Column("project_id")]
        public int ProjectId { get; set; }
        [Column("task_id")]
        public int TaskId { get; set; }
        [Column("group_id")]
        public int? GroupId { get; set; }
        [Column("org_id")]
        public int? OrgId { get; set; }
    }
}
