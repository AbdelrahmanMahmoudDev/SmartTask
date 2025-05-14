namespace SmartTask.Api.DTOs.BranchDto
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public List<int> DepartmentIds { get; set; } = new();
    }

}
