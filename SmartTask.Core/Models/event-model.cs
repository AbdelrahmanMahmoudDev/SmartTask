using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        [Required]
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        [Required]
        [StringLength(255)]
        public string Subject { get; set; }

        public string Attendees { get; set; }

        public string ImportedById { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        [ForeignKey("ImportedById")]
        public virtual ApplicationUser ImportedBy { get; set; }
    }
}
