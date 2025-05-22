using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels.DepartmentVM
{
    public class DepartmentFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Display(Name = "Department Manager")]
        public string ManagerId { get; set; }

        public List<string> SelectedUserIds { get; set; }
        public List<int> SelectedBranchIds { get; set; } = new List<int>();
    }
}
