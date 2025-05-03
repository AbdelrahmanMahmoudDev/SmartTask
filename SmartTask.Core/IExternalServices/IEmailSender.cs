using SmartTask.Core.Models.Mail;

namespace SmartTask.Core.IExternalServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}