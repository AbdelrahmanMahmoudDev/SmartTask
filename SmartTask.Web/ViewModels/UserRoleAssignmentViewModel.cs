

namespace SmartTask.Core.ViewModels
{
    public class UserRoleAssignmentViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public List<RoleCheckbox> Roles { get; set; } = new();
    }

    public class RoleCheckbox
    {
        public string RoleName { get; set; }
        public bool IsAssigned { get; set; }
    }
}