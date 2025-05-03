namespace SmartTask.Web.Models
{
    public class Notification
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? UserId { get; set; }  // Recipient user ID (null for broadcast)
        public string Type { get; set; } = "info";  // info, success, warning, error

        // Empty constructor for deserialization
        public Notification() { }

        public Notification(string message, string type = "info", string? userId = null)
        {
            Message = message;
            Type = type;
            UserId = userId;
        }
    }
}
