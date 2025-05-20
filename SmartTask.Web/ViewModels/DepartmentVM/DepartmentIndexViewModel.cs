using SmartTask.Bl.Helpers;
using SmartTask.Core.Models;

namespace SmartTask.Web.ViewModels.DepartmentVM
{
    public class DepartmentIndexViewModel
    {
        public PaginatedList<Department> Departments { get; set; }
        public string SearchString { get; set; }
    }
}
