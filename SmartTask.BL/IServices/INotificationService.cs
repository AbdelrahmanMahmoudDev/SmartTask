using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.IServices
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string to, string subject, string body);
    }
}
