using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class AISuggestion
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        [Required]
        [StringLength(100)]
        public string SuggestionType { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }
    }
}
