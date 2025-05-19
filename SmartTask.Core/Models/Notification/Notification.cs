using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.Models.Notification
{
    public class Notification
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string Type { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? CreatedAt { get; set; }

        public bool? IsRead { get; set; }

        public string? link { get; set; }


        public string? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public ApplicationUser? Sender { get; set; }

        // Foreign Key for Receiver
        public string? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser? Receiver { get; set; }


    }


}
