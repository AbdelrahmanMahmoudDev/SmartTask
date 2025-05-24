namespace SmartTask.Web.ViewModels.DashboardVM
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DepartmentViewModel Department { get; set; }
        public IEnumerable<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
        public double CompletionPercentage { get; set; }
    }
}
