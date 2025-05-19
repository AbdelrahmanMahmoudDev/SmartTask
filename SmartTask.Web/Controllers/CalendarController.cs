using Microsoft.AspNetCore.Mvc;

namespace SmartTask.Web.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
