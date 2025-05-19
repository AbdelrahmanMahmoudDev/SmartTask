using Microsoft.AspNetCore.SignalR;
using SmartTask.BL.IServices;
using SmartTask.BL.Service.Hubs;
using SmartTask.Core.IExternalServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.Mail;
using SmartTask.Core.Models.Notification;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.Bl.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IEmailSender emailSender,
            INotificationRepository notificationRepository, IHubContext<NotificationHub> hub)
        {
            _emailSender = emailSender;
            _notificationRepository = notificationRepository;
            _hub = hub;
        }

        public async Task SendNotificationAsync(string to, string subject, string body)
        {
            var message = new EmailMessage
            {
                To = to,
                Subject = subject,
                Body = body
            };

            await _emailSender.SendEmailAsync(message);
        }

        private string GenerateNotificationLink(string notificationType)
        {
            // Generate links based on notification type
            switch (notificationType.ToLower())
            {
                case "newtask":
                    return $"/Task"; // Data would contain the taskId
                case "updatetask":                          
                    return $"/Task";
                case "projectupdate":                       
                    return $"/Task";
                case "mention":
                    return $"/Task";
                case "comment":
                    return $"/Task/Details/{notificationType}#comment-section";
                case "delete":
                    return "#"; // No link for delete notifications
                default:
                    return "#";
            }
        }

        public async Task sendSignalRNotificationAsync(List<string>receivers,string sender,
            string notificationType, string notificationMessage)
        {
            Notification notification;
            bool flag = true;
            string url = GenerateNotificationLink(notificationType);
            foreach (var receiver in receivers)
            {
                notification = new Notification
                {
                    SenderId = sender,
                    ReceiverId = receiver,
                    Type = notificationType,
                    Message = notificationMessage,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    link = url
                };
                // Send notification to SignalR
                if (flag)
                {
                    await _hub.Clients.Users(receivers).SendAsync("assignedtask", notification);

                    flag = false;
                }
                // Save notification to database
                 _notificationRepository.AddAsync(notification);
            }
        }
        public async Task<List<Notification>> GetUserNotificationsAsync(string userid) 
        {
            return await _notificationRepository.GetAllWithReceiverIdAsync(userid);
        }

        public async Task MarkAllNotificationAsReadAsync(string userid)
        {
            await _notificationRepository.MarkAllAsReadAsync(userid);
        }

        public async Task MarkNotificationAsReadAsync(int NotificationId)
        {
            await _notificationRepository.MarkAsReadAsync(NotificationId);
        }

    }
}