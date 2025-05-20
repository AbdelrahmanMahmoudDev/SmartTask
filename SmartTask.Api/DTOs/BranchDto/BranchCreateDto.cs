using System.ComponentModel.DataAnnotations;

namespace SmartTask.Api.DTOs.BranchDto
{
    public class BranchCreateDto
    {
        [Required(ErrorMessage = "Branch Name Required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string ManagerId { get; set; }

        public List<int> DepartmentIds { get; set; } = new();
    }
}
