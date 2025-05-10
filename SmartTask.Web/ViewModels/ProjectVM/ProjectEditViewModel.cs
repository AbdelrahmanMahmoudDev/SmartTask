using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels.ProjectVM
{
    public class ProjectEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project Name is Required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Please select owner")]
        [Display(Name = "Project Owner")]
        public string OwnerId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public List<UserCheckboxModel> AssignedUsers { get; set; } = new List<UserCheckboxModel>();
    }

    public class UserCheckboxModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public bool IsChecked { get; set; }
    }
}
