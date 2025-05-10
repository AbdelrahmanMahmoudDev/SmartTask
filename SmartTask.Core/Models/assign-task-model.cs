using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartTask.Core.Models.Enums;

namespace SmartTask.Core.Models
{
    public class AssignTask
    {
        [Key, Column(Order = 0)]
        public int TaskId { get; set; }

        [Key, Column(Order = 1)]
        public string UserId { get; set; }

        public string AssignedById { get; set; }

        [Required]
        public DateTime AssignedAt { get; set; }

    
        public Status Status { get; set; }//Should be deleted !!!!!!!!!!!!!!!

        public string Comments { get; set; }//Should be a Navigation property to a comment model

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AssignedById")]
        public virtual ApplicationUser AssignedBy { get; set; }
    }
}
