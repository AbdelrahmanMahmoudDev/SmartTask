using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{ 
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? ManagerId { get; set; }

        // Navigation properties
        [ForeignKey("ManagerId")]
        public virtual ApplicationUser Manager { get; set; }

        public virtual ICollection<BranchDepartment> BranchDepartments { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Department()
        {
            BranchDepartments = new HashSet<BranchDepartment>();
            Users = new HashSet<ApplicationUser>();
        }
    }
}
