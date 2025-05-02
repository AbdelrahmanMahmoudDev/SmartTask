using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }
        public int AuthorId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }
    }
}
