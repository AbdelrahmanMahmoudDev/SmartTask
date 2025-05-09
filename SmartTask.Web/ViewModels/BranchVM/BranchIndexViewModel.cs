using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTask.Bl.Helpers;
using SmartTask.Core.Models;

namespace SmartTask.Web.ViewModels.BranchVM
{
    public class BranchIndexViewModel
    {
        public PaginatedList<Branch> Branches { get; set; }
        public string SearchString { get; set; }
        public string? ManagerId { get; set; }
        public SelectList Managers { get; set; }
    }
}
