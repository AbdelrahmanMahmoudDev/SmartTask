using SmartTask.BL.IServices;
using SmartTask.Core.IExternalServices;
using SmartTask.Core.Models.Mail;

namespace SmartTask.Bl.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;

        public NotificationService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
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
    }
}