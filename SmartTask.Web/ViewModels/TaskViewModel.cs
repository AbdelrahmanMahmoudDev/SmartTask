using SmartTask.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TaskModel = SmartTask.Core.Models.Task;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SmartTask.Web.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int? ParentTaskId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

    
        public string Status { get; set; }

        [StringLength(50)]
        public string Priority { get; set; }

        public string? CreatedById { get; set; }
        public string? UpdatedById { get; set; }

        [Required]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
    
        public virtual string? ProjectName { get; set; }


        [Display(Name ="Parent Task")]
        public virtual TaskModel? ParentTask { get; set; }

        [Display(Name ="Created By")]
        public virtual ApplicationUser? CreatedBy { get; set; }
        [Display(Name ="Updated By")]
        public virtual ApplicationUser? UpdatedBy { get; set; }

        public virtual ICollection<TaskModel>? SubTasks { get; set; }= new List<TaskModel>();
        //public virtual ICollection<TaskDependency> PredecessorDependencies { get; set; }
        //public virtual ICollection<TaskDependency> SuccessorDependencies { get; set; }
        //public virtual ICollection<Comment> Comments { get; set; }
        //public virtual ICollection<Attachment> Attachments { get; set; }
        //public virtual ICollection<Event> Events { get; set; }
        //public virtual ICollection<AISuggestion> AISuggestions { get; set; }
        public virtual List<string> AssignedToId { get; set; }=new List<string>();

    }
}
