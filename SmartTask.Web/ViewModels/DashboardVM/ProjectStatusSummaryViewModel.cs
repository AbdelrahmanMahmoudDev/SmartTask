namespace SmartTask.Web.ViewModels.DashboardVM
{
    public class ProjectStatusSummaryViewModel
    {
        public int TotalProjects { get; set; }
        public int PendingProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int OverdueProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletionPercentage { get; set; }
        public int InProgressPercentage { get; set; }
        public int PendingPercentage { get; set; }
        public int TaskPerProject { get; set; }
    }
}
