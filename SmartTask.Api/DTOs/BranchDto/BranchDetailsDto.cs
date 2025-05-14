using SmartTask.Api.DTOs.Department;

namespace SmartTask.Api.DTOs.BranchDto
{
    public class BranchDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }
        public List<DepartmentDto> Departments { get; set; } = new();
    }
}
