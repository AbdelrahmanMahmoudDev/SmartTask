using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models.Notification;
using SmartTask.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTask.Core.IRepositories;
namespace SmartTask.DataAccess.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly SmartTaskContext _context;

        public NotificationRepository(SmartTaskContext context)
        {
            _context = context;
        }
        public Notification AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
             _context.SaveChanges();
            return notification;
        }

        public async Task<List<Notification>> GetAllWithReceiverIdAsync(string id)
        {
            return await _context.Notifications
                .Where(notification => notification.ReceiverId == id)
                .ToListAsync();
        }

        public async Task MarkAllAsReadAsync(string id)
        {
            var notifications = await GetAllWithReceiverIdAsync(id);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            _context.Notifications.UpdateRange(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
