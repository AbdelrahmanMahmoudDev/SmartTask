using Microsoft.Build.Evaluation;
using SmartTask.Bl.Helpers;
using Project =  SmartTask.Core.Models.Project;

namespace SmartTask.Web.ViewModels.ProjectVM
{
    public class ProjectIndexViewModel
    {
        public PaginatedList<Project> Projects { get; set; }
        public string SearchString { get; set; }
    }
}
