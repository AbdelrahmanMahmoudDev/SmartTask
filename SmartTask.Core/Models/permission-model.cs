using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<ProjectRolePermission> ProjectRolePermissions { get; set; }

        public Permission()
        {
            RolePermissions = new HashSet<RolePermission>();
            ProjectRolePermissions = new HashSet<ProjectRolePermission>();
        }
    }
}
