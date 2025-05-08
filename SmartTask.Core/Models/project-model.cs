using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        public string OwnerId { get; set; }
        public string CreatedById { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        //public virtual ICollection<ProjectRole> ProjectRoles { get; set; }
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }

        public Project()
        {
            //ProjectRoles = new HashSet<ProjectRole>();
            CreatedAt = DateTime.Now;
            ProjectMembers = new HashSet<ProjectMember>();
            Tasks = new HashSet<Task>();
        }
    }
}
