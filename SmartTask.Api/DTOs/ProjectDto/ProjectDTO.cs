namespace SmartTask.Api.DTOs.ProjectDto
{
    public class ProjectDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "pending";
        public int? DepartmentId { get; set; }
        public int? BranchId { get; set; }
        public string OwnerId { get; set; }

        public OwnerDTO? Owner { get; set; }
    }

    public class OwnerDTO
    {
        public string FullName { get; set; }
    }
}
