using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class ProjectRolePermission
    {
        [Key, Column(Order = 0)]
        public int ProjectRoleId { get; set; }

        [Key, Column(Order = 1)]
        public int PermissionId { get; set; }

        // Navigation properties
        [ForeignKey("ProjectRoleId")]
        public virtual ProjectRole ProjectRole { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
    }
}
