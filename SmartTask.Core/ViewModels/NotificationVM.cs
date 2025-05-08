using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTask.Core.ViewModels
{
    public class NotificationVM
    {
        public string Message { get; set; }

        
        public string Sender { get; set; }
    }
}
