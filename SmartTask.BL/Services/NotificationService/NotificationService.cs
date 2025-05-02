using SmartTask.BL.IServices;
using SmartTask.Core.IExternalServices;
using SmartTask.Core.Models.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.Services.NotificationService
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
