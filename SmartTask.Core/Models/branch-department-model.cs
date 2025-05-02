using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class BranchDepartment
    {
        [Key, Column(Order = 0)]
        public int BranchId { get; set; }

        [Key, Column(Order = 1)]
        public int DepartmentId { get; set; }

        // Navigation properties
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
    }
}
