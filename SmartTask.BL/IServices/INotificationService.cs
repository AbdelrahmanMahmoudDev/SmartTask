namespace SmartTask.BL.IServices
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string to, string subject, string body);
    }
}