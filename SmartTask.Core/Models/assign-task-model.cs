using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column(TypeName = "Date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public string Comments { get; set; }

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AssignedById")]
        public virtual ApplicationUser AssignedBy { get; set; }
    }
}