using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
//using SmartTask.Web.Models;
//using SignalRNotifications.Hubs;
using SmartTask.Web.Controllers;

using SmartTask.BL.Service.Hubs;

namespace SmartTask.Web.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
