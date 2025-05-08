using SmartTask.Core.Models.BasePermission;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Core.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.")]
        public string Name { get; set; }

        public IEnumerable<MvcControllerInfo> SelectedControllers { get; set; }
    }
}
