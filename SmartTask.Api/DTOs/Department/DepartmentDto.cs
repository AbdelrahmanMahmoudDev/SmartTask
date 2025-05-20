using System.ComponentModel.DataAnnotations;

namespace SmartTask.Api.DTOs.Department
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
    }
    public class DepartmentCreateDto
    {
        [Required]
        public string Name { get; set; }

        public string ManagerId { get; set; }

        public List<string> SelectedUserIds { get; set; } = new();
    }

    public class DepartmentUpdateDto
    {
        [Required]
        public string Name { get; set; }

        public string ManagerId { get; set; }

        public List<string> SelectedUserIds { get; set; } = new();
    }

    public class DepartmentDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }
        public List<UserDto> Users { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

}
