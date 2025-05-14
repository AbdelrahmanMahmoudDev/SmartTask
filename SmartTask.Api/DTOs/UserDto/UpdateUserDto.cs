namespace SmartTask.Web.Dto
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
    }
}
