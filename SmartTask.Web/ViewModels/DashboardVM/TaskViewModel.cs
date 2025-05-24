using SmartTask.Core.Models.Enums;

namespace SmartTask.Web.ViewModels.DashboardVM
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public string AssignedToName { get; set; }
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Today && Status != Status.Done;
    }
}
