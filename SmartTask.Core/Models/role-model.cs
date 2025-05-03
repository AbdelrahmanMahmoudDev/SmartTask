using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Role()
        {
            RolePermissions = new HashSet<RolePermission>();
            Users = new HashSet<ApplicationUser>();
        }
    }
}
