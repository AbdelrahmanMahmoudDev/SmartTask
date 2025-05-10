using System.ComponentModel.DataAnnotations;

namespace SmartTask.Core.ViewModels
{
    public class UserLoginHistoryViewModel
    {

        public int Id { get; set; }
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Login Time")]
        public DateTime LoginTime { get; set; }
        [Display(Name = "IP Address")]
        public string IPAddress { get; set; }
        [Display(Name = "User Agent")]
        public string UserAgent { get; set; }
    }
}
