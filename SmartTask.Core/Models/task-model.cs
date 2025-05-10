using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartTask.Core.Models.Enums;
using TaskModel = SmartTask.Core.Models.Task;


namespace SmartTask.Core.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int? ParentTaskId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? EndDate { get; set; }

    
        public Status Status { get; set; }

       
        public Priority Priority { get; set; }

        public string CreatedById { get; set; }
        public string? UpdatedById { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }


        [ForeignKey("ParentTaskId")]
        public virtual Task ParentTask { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual ApplicationUser UpdatedBy { get; set; }

        public virtual ICollection<Task> SubTasks { get; set; }
        public virtual ICollection<TaskDependency> PredecessorDependencies { get; set; }
        public virtual ICollection<TaskDependency> SuccessorDependencies { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<AISuggestion> AISuggestions { get; set; }
        public virtual ICollection<AssignTask> Assignments { get; set; }

        public Task()
        {
            SubTasks = new HashSet<Task>();
            PredecessorDependencies = new HashSet<TaskDependency>();
            SuccessorDependencies = new HashSet<TaskDependency>();
            Comments = new HashSet<Comment>();
            Attachments = new HashSet<Attachment>();
            Events = new HashSet<Event>();
            AISuggestions = new HashSet<AISuggestion>();
            Assignments = new HashSet<AssignTask>();
        }
    }
}
