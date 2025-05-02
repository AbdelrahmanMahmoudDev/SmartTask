using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using SmartTask.Core.Models.Mail;
using Microsoft.Extensions.Options;
using SmartTask.Core.IExternalServices;

namespace SmartTask.DataAccess.ExternalServices.EmailService
{
    public class EmailService : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }


        public async Task SendEmailAsync(EmailMessage message)
        {

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            email.To.Add(new MailboxAddress("", message.To));
            email.Subject = message.Subject;
            email.Body = new TextPart("html") { Text = message.Body };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_smtpSettings.SenderEmail, _smtpSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
