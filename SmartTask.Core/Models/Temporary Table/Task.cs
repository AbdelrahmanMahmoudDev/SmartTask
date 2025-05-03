using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models.TemporaryTable
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; } // 1-5
        public int? ProjectId { get; set; } // Foreign key to Project
        public string? UserId { get; set; } // Foreign key to User
        public virtual ApplicationUser? User { get; set; }
    }
}
