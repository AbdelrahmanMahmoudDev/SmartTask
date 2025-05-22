using SmartTask.Core.Models.Notification;

namespace SmartTask.BL.IServices
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string to, string subject, string body);

        Task sendSignalRNotificationAsync(List<string> receivers, string sender,
            string notificationType, string notificationMessage, int notifyForID);
        Task<List<Notification>> GetUserNotificationsAsync(string userid);
        Task MarkAllNotificationAsReadAsync(string userid);
        Task MarkNotificationAsReadAsync(int NotificationId);


    }
}