using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Security.Labels.Categories.Item.Subcategories.Item;
using SmartTask.Core.IRepositories;
using SmartTask.Web.ViewModels;
using System.Threading.Tasks;

namespace SmartTask.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public CalendarController(ITaskRepository taskRepository,IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }
        public async Task<IActionResult> Index()
        {
          
           
            ViewBag.Projects=await _projectRepository.GetAllAsyncWithoutInclude();
           
            return View();
        }
        public async Task<IActionResult> GetProjectById(int ProjectId)
        
        {
            var project = await _projectRepository.GetByIdAsync(ProjectId);
            var Duration = new { StartDate=project.StartDate?.ToString("yyyy-MM-dd"), EndDate= project.EndDate?.ToString("yyyy-MM-dd") };
            return Json(Duration);
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
                    Start = task.StartDate?.ToString("yyyy-MM-dd"),
                    End = task.EndDate?.ToString("yyyy-MM-dd"),
                   TaskStatus = task.Status
                };
                CalendarViewModels.Add(taskVM);
            }
            return Json(CalendarViewModels);
        }
    }
}
