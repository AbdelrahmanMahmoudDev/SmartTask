using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class ProjectMember
    {
        [Key, Column(Order = 0)]
        public int ProjectId { get; set; }

        [Key, Column(Order = 1)]
        public string UserId { get; set; }

        //public int ProjectRoleId { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        //[ForeignKey("ProjectRoleId")]
        //public virtual ProjectRole ProjectRole { get; set; }
    }
}
