using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTask.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels.BranchVM
{
    public class BranchFormViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Branch Name Required")]
        [StringLength(100, ErrorMessage = "Not more than 100 characters")]
        public string Name { get; set; }

        [Display(Name = "Manager")]
        public string ManagerId { get; set; }

        [Display(Name = "Departments")]
        public List<int>? SelectedDepartmentIds { get; set; } 

        public IEnumerable<Department>? AllDepartments { get; set; }
        [Display(Name = "Assigned Users")]
        public List<string>? SelectedUserIds { get; set; } = new List<string>();

        public BranchFormViewModel()
        {
            AllDepartments = new List<Department>();
        }
    }
}
