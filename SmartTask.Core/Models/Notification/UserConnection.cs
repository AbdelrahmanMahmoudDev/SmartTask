using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models.Notification
{
    public class UserConnection
    {

        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public ApplicationUser applicationUser { get; set; }

        public string connectionID { get; set; }
    }
}

//[ForeignKey("applicationUser")]
