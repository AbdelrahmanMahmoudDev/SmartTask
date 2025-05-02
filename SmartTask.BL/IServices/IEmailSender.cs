using SmartTask.Core.Models.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.IServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
