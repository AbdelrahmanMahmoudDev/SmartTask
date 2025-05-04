using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels;

namespace SmartTask.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationService notificationService;
        private readonly ITaskRepository taskRepository;

        public HomeController(ILogger<HomeController> logger, INotificationService notificationService, ITaskRepository taskRepository)
        {
            _logger = logger;
            this.notificationService = notificationService;
            this.taskRepository = taskRepository;
        }

        public async Task<IActionResult> IndexAsync()
        {
            
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Core.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateTask(SmartTask.Core.Models.Task task)
        {
            taskRepository.AddAsync(task);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> IndexTask()
        {
            var Tasks = await taskRepository.GetAllAsync();
            return View(Tasks);
        }
    }
}