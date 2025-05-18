using SmartTask.Core.Models;

namespace SmartTask.Web.ViewModels.DashboardVM
{
    public class RecentProjectsViewModel
    {
        public List<Project> Projects { get; set; } = new List<Project>();
        public int Count { get; set; }
     
    }
}
