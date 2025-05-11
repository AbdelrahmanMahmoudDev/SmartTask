using SmartTask.Core.Models;
using SmartTask.Core.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Core.IRepositories
{
    public interface INotificationRepository
    {
        Task<Notification> AddAsync(Notification notification);

        Task<IEnumerable<Notification>> GetAllWithReceiverIdAsync(string id);
    }
}
