using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
//using SmartTask.Web.Models;
//using SignalRNotifications.Hubs;
using SmartTask.Web.Controllers;

using SmartTask.BL.Service.Hubs;
using SmartTask.BL.IServices;
using Microsoft.AspNetCore.Identity;
using SmartTask.Core.Models;

namespace SmartTask.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> usermanager)
        {
            _notificationService = notificationService;
            _userManager = usermanager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User); // Or however you get the current user ID
                var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
                var hasUnread = notifications.Any(n => (bool)!n.IsRead);

                return Json(new
                {
                    success = true,
                    notifications = notifications.Select(n => new
                    {
                        id = n.Id,
                        message = n.Message,
                        type = n.Type,
                        timestamp = n.CreatedAt,
                        isRead = n.IsRead,
                        link = n.link // If you have a link property
                    }),
                    hasUnread = hasUnread
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to load notifications" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);  // Or however you get the current user ID
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }
                await _notificationService.MarkAllNotificationAsReadAsync(user.Id);

                return Json(new { success = true, message = "All notifications marked as read" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to mark notifications as read" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                //var userId = User.Identity.Name; // Or however you get the current user ID
                await _notificationService.MarkNotificationAsReadAsync(notificationId);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to mark notification as read" });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
