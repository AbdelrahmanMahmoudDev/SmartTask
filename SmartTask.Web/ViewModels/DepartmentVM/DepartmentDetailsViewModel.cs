namespace SmartTask.Web.ViewModels.DepartmentVM
{
    public class DepartmentDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }
        public List<UserViewModel> Users { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
