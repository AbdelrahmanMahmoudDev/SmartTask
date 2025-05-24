using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartTask.Web.Models;
//using SignalRNotifications.Hubs;
using SmartTask.Web.Controllers;
using SmartTask.Bl.Hubs;

namespace SignalR.Controllers
//using SmartTask.BL.Service.Hubs;

namespace SmartTask.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(IHubContext<NotificationHub> hubContext,ILogger<NotificationController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public IActionResult Index() 
        {
            // For demo purposes, let's set up a mock user ID if not logged in
            if (string.IsNullOrEmpty(User.Identity?.Name))
            {
                ViewBag.UserId = "demo-user-" + new Random().Next(1000, 9999);
            }
            else
            {
                ViewBag.UserId = User.Identity.Name;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(string message, string type, string? userId = null)
        {
            var notification = new Notification
            {
                Message = message,
                Type = string.IsNullOrEmpty(type) ? "info" : type,
                UserId = userId,
                Timestamp = DateTime.Now
            };

            if (string.IsNullOrEmpty(userId))
            {
                // Broadcast to all connected clients
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
                _logger.LogInformation($"Broadcast notification: {notification.Message}");
            }
            else
            {
                // Send to specific user
                await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation($"Notification sent to user {userId}: {notification.Message}");
            }

            return Ok(new { Success = true });
        }
    }
}
