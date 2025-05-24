using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models
{
    public class TaskDependency
    {
        [Key]
        public int Id { get; set; }

        public int PredecessorId { get; set; }
        public int SuccessorId { get; set; }

        public DependencyType DependencyType { get; set; }

        // Navigation properties
        [ForeignKey("PredecessorId")]
        public virtual Task Predecessor { get; set; }

        [ForeignKey("SuccessorId")]
        public virtual Task Successor { get; set; }
    }
    public enum DependencyType:byte
    {
        FinishToStart = 1,
        StartToStart = 2
      
    }
}
