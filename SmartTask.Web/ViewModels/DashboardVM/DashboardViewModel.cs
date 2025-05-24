using SmartTask.Core.Models;

namespace SmartTask.Web.ViewModels.DashboardVM
{
    public class DashboardViewModel
    {
        // Project Statistics
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedProjects { get; set; }
        public int PendingProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int ArchievedProjects { get; set; }
        public int CancelledProjects { get; set; }

        // Collections
        public IEnumerable<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();
        public IEnumerable<TaskViewModel> MyTasks { get; set; } = new List<TaskViewModel>();
        public IEnumerable<ProjectViewModel> RecentProjects { get; set; } = new List<ProjectViewModel>();

        // Task Statistics
        public int TodoTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int CancelledTasks { get; set; }
        public int ArchievedTasks { get; set; }

        // Calculated properties
        public double CompletionRate => TotalProjects > 0 ? (CompletedProjects * 100.0 / TotalProjects) : 0;
        public double InProgressRate => TotalProjects > 0 ? (InProgressProjects * 100.0 / TotalProjects) : 0;
        public double PendingRate => TotalProjects > 0 ? (PendingProjects * 100.0 / TotalProjects) : 0;
        public double ArchievedRate => TotalProjects > 0 ? (ArchievedProjects * 100.0 / TotalProjects) : 0;
        public double CancelledRate => TotalProjects > 0 ? (CancelledProjects * 100.0 / TotalProjects) : 0;
        public double TaskCoverage => TotalProjects > 0 ? (TotalTasks * 100.0 / TotalProjects) : 0;
    }
}
