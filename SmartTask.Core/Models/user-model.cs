using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        public int? RoleId { get; set; }
        public int? DepartmentId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public virtual ICollection<Branch> ManagedBranches { get; set; }
        public virtual ICollection<Department> ManagedDepartments { get; set; }
        public virtual ICollection<Project> OwnedProjects { get; set; }
        public virtual ICollection<Project> CreatedProjects { get; set; }
        public virtual ICollection<ProjectMember> ProjectMemberships { get; set; }
        public virtual ICollection<Task> CreatedTasks { get; set; }
        public virtual ICollection<Task> UpdatedTasks { get; set; }
        public virtual ICollection<Task> AssignedTasks { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Event> ImportedEvents { get; set; }
        public virtual ICollection<AssignTask> TaskAssignments { get; set; }
        public virtual ICollection<AssignTask> TasksAssigned { get; set; }

        public User()
        {
            ManagedBranches = new HashSet<Branch>();
            ManagedDepartments = new HashSet<Department>();
            OwnedProjects = new HashSet<Project>();
            CreatedProjects = new HashSet<Project>();
            ProjectMemberships = new HashSet<ProjectMember>();
            CreatedTasks = new HashSet<Task>();
            UpdatedTasks = new HashSet<Task>();
            AssignedTasks = new HashSet<Task>();
            Comments = new HashSet<Comment>();
            Attachments = new HashSet<Attachment>();
            ImportedEvents = new HashSet<Event>();
            TaskAssignments = new HashSet<AssignTask>();
            TasksAssigned = new HashSet<AssignTask>();
        }
    }
}
