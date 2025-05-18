using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models.Notification
{
    public class UserGroups
    {
        [ForeignKey("applicationUser")]
        public string UserID { get; set; }
        public ApplicationUser applicationUser { get; set; }


        [ForeignKey("groups")]
        public int GroupID { get; set; }
        public Groups groups { get; set; }
    }
}
