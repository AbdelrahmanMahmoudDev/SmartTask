using SmartTask.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Api.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public string OutLooktTaskId { get; set; }
        public int? TaskId { get; set; }

        [Required]
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        [Required]
        [StringLength(255)]
        public string Subject { get; set; }

        public string Attendees { get; set; }

        public string ImportedById { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual string projectname { get; set; }
    }
}
