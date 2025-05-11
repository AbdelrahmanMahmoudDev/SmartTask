using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models.Notification;
using SmartTask.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.DataAccess.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly SmartTaskContext _context;

        public NotificationRepository(SmartTaskContext context)
        {
            _context = context;
        }
        public async Task<Notification> AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<IEnumerable<Notification>> GetAllWithReceiverIdAsync(string id)
        {
            return await _context.Notifications
                .Where(notification => notification.ReceiverId == id)
                .ToListAsync();
        }
    }
}
