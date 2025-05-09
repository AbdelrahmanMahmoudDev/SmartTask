using Microsoft.Build.Evaluation;
using SmartTask.Bl.Helpers;
using SmartTask.Core.Models;
using Project =  SmartTask.Core.Models.Project;

namespace SmartTask.Web.ViewModels.ProjectVM
{
    public class ProjectIndexViewModel
    {
        public PaginatedList<Project> Projects { get; set; }
        public string SearchString { get; set; }

        public int? SelectedDepartmentId { get; set; }
        public int? SelectedBranchId { get; set; }

        public List<Department> Departments { get; set; }
        public List<Branch> Branches { get; set; }
    }

}
