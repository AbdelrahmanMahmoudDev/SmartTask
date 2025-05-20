using Microsoft.AspNetCore.Mvc;
using SmartTask.Core.IRepositories;
using SmartTask.Web.ViewModels;
using System.Threading.Tasks;

namespace SmartTask.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public CalendarController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTasksForProject(int ProjectId)
        {
            var tasks = await _taskRepository.GetAllTasksPerProject(ProjectId);
            var CalendarViewModels = new List<CalendarViewModel>();
            foreach (var task in tasks)
            {
                var taskVM = new CalendarViewModel()
                {
                    Id = task.Id,
                    Title = task.Title,
                    Start = task.StartDate,
                    End = task.EndDate
                };
                CalendarViewModels.Add(taskVM);
            }
            return Json(CalendarViewModels);
        }
    }
}
