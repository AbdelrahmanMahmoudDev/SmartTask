using Microsoft.AspNetCore.Identity;

namespace SmartTask.Web.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}
