using SmartTask.Core.Models;

namespace SmartTask.Web.ViewModels.DashboardVM
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedProjects { get; set; }
        public int PendingProjects { get; set; }
        public int InProgressProjects { get; set; }

        // Calculated properties
        public double CompletionRate => TotalProjects > 0 ? (CompletedProjects * 100.0 / TotalProjects) : 0;
        public double InProgressRate => TotalProjects > 0 ? (InProgressProjects * 100.0 / TotalProjects) : 0;
        public double PendingRate => TotalProjects > 0 ? (PendingProjects * 100.0 / TotalProjects) : 0;
        public double TaskCoverage => TotalProjects > 0 ? (TotalTasks * 100.0 / TotalProjects) : 0;

    }
}
