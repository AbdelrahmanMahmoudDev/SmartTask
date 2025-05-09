using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models.Notification
{
    public class Notification
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string Type { get; set; }

        [ForeignKey("applicationUser")]
        public string Sender { get; set; }
        public ApplicationUser? applicationUser { get; set; }
    }
}
