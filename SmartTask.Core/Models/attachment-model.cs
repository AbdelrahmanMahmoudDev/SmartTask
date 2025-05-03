using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        [StringLength(1000)]
        public string FilePath { get; set; }

        public string UploadedById { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        [ForeignKey("UploadedById")]
        public virtual ApplicationUser UploadedBy { get; set; }
    }
}
