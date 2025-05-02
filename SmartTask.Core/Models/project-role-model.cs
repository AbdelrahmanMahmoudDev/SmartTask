using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{ 
    public class ProjectRole
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public virtual ICollection<ProjectRolePermission> ProjectRolePermissions { get; set; }
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }

        public ProjectRole()
        {
            ProjectRolePermissions = new HashSet<ProjectRolePermission>();
            ProjectMembers = new HashSet<ProjectMember>();
        }
    }
}
