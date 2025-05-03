using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SmartTask.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Bl.Hubs
{
    public class NotificationHub:Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            // First try authenticated user
            var userId = Context.User?.Identity?.Name;

            // For the demo application, we'll handle anonymous connections too
            await base.OnConnectedAsync();
        }

        // Client method to associate a user ID with the connection
        public async Task RegisterUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                // Add connection to the group with user's ID
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation($"User {userId} connected with connection ID: {Context.ConnectionId}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // We don't need to explicitly remove from groups
            // SignalR automatically removes connections from groups when they disconnect

            _logger.LogInformation($"Connection {Context.ConnectionId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        // This method can be called from client-side
        public async Task SendNotification(Notification notification)
        {
            if (string.IsNullOrEmpty(notification.UserId))
            {
                // Broadcast to all connected clients
                await Clients.All.SendAsync("ReceiveNotification", notification);
                _logger.LogInformation($"Broadcast notification: {notification.Message}");
            }
            else
            {
                // Send to specific user
                await Clients.Group(notification.UserId).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation($"Notification sent to user {notification.UserId}: {notification.Message}");
            }
        }
    }
}
