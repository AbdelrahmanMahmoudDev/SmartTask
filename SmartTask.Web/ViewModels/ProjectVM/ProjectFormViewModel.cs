using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTask.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels.ProjectVM
{
    public class ProjectFormViewModel
    {
        [Required(ErrorMessage ="Project Name is Required")]
        [StringLength(50, ErrorMessage = " not over 50 ch")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Please select owner")]
        [Display(Name = "Project owner")]
        public string OwnerId { get; set; }
        public int? SelectedDepartmentId { get; set; }
       
        public int? SelectedBranchId { get; set; }
        
       



    }
}
