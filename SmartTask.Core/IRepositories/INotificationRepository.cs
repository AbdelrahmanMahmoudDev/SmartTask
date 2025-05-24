using SmartTask.Core.Models;
using SmartTask.Core.Models.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.Core.IRepositories
{
    public interface INotificationRepository
    {
        Notification AddAsync(Notification notification);

        Task<List<Notification>> GetAllWithReceiverIdAsync(string id);

        Task MarkAllAsReadAsync(string id);
        Task MarkAsReadAsync(int id);
    }
}
