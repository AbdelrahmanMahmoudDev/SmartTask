using SmartTask.Bl.Helpers;
using SmartTask.Core.Models;

namespace SmartTask.Web.ViewModels
{
    public class UsersViewModel
    {
        public PaginatedList<ApplicationUser> Users { get; set; }
        public string? SearchString { get; set; }
    }
}
