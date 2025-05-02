using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{ 
    public class RolePermission
    {
        [Key, Column(Order = 0)]
        public int RoleId { get; set; }

        [Key, Column(Order = 1)]
        public int PermissionId { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
    }
}
