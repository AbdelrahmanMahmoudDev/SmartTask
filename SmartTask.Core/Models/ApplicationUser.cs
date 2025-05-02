using Microsoft.AspNetCore.Identity;

namespace SmartTask.Core.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }

        public DateTime createdAt { get; set; }

        public DateTime updatedAt { get; set; }
    }
}