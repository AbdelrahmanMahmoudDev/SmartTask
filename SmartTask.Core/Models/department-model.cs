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

        public int? ManagerId { get; set; }

        // Navigation properties
        [ForeignKey("ManagerId")]
        public virtual User Manager { get; set; }

        public virtual ICollection<BranchDepartment> BranchDepartments { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Department()
        {
            BranchDepartments = new HashSet<BranchDepartment>();
            Users = new HashSet<User>();
        }
    }
}
