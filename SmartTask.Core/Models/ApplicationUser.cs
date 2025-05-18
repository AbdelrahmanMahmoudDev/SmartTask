using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string? ImageUrl { get; set; }
        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }

        public int? DepartmentId { get; set; }

        public int? BranchId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Branch> ManagedBranches { get; set; } = new List<Branch>();
        public virtual ICollection<Department> ManagedDepartments { get; set; } = new List<Department>();
        public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
        public virtual ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
        public virtual ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
        public virtual ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
        public virtual ICollection<Task> UpdatedTasks { get; set; } = new List<Task>();
        public virtual ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public virtual ICollection<Event> ImportedEvents { get; set; } = new List<Event>();
        public virtual ICollection<AssignTask> TaskAssignments { get; set; } = new List<AssignTask>();
        public virtual ICollection<AssignTask> TasksAssigned { get; set; } = new List<AssignTask>();
    }
}