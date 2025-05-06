using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels.BranchVM
{
    public class BranchFormViewModel
    {

        [Required(ErrorMessage = "Branch Name Required")]
        [StringLength(100, ErrorMessage = "Not more than 100 characters")]
        public string Name { get; set; }

        [Display(Name = "Manager")]
        public int? ManagerId { get; set; }

        public SelectList Managers { get; set; } 
    }
}
