using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels;
using TaskModel = SmartTask.Core.Models.Task;

namespace SmartTask.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private UserManager<ApplicationUser> _userManager;
        public TaskController(ITaskRepository taskRepository,IProjectRepository projectRepository,UserManager<ApplicationUser>usermanager)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userManager = usermanager;
        }
        public async Task<IActionResult> Index()
        {
            List<TaskViewModel> taskViewModels = new List<TaskViewModel>();
            var tasks = await _taskRepository.GetAllAsync();
            foreach(var task in tasks)
            {
                TaskViewModel taskVM = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    CreatedById = task.CreatedById,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    ProjectId = task.ProjectId,
                    AssignedToId = task.AssignedToId,
                    ParentTaskId = task.ParentTaskId,
                    UpdatedById = task.UpdatedById,
                    ProjectName=task.Project.Name
                };
                taskViewModels.Add(taskVM);
            }
            return View(taskViewModels);
        }
        [HttpGet]
      
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _userManager.Users.ToListAsync();
            ViewBag.Projects = await _projectRepository.GetAllAsyncWithoutInclude();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel taskVM)
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value;
            taskVM.CreatedById = userId;
            if (!ModelState.IsValid)
            {
                ViewBag.Users = await _userManager.Users.ToListAsync();

                ViewBag.Projects = await _projectRepository.GetAllAsyncWithoutInclude();
                return View(taskVM);

            }
            TaskModel task = new TaskModel
            {
                Title = taskVM.Title,
                Description = taskVM.Description,
                StartDate = taskVM.StartDate,
                EndDate = taskVM.EndDate,
                Status = taskVM.Status,
                Priority = taskVM.Priority,
                CreatedById =userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ProjectId = taskVM.ProjectId,
                AssignedToId = taskVM.AssignedToId,
                ParentTaskId = taskVM.ParentTaskId,
                UpdatedById = userId,
            };
            await _taskRepository.AddAsync(task);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute]int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var taskVM = new TaskViewModel()
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                Status = task.Status,
                Priority = task.Priority,
                CreatedById = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ProjectId = task.ProjectId,
                AssignedToId = task.AssignedToId,
                ParentTaskId = task.ParentTaskId,
                UpdatedById = userId,
            };
            var tasks = await GetTaskByProject(task.ProjectId);
            ViewBag.Tasks = JsonSerializer.Deserialize<List<TaskViewModel>>(JsonSerializer.Serialize(tasks.Value));
            ViewBag.Users = await _userManager.Users.ToListAsync();
            ViewBag.Projects = await _projectRepository.GetAllAsyncWithoutInclude();
            return View(taskVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskViewModel taskVM)
        {
            if (ModelState.IsValid) 
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                TaskModel task = new TaskModel
                {
                    Id= taskVM.Id,
                    Title = taskVM.Title,
                    Description = taskVM.Description,
                    StartDate = taskVM.StartDate,
                    EndDate = taskVM.EndDate,
                    Status = taskVM.Status,
                    Priority = taskVM.Priority,
                    CreatedById = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ProjectId = taskVM.ProjectId,
                    AssignedToId = taskVM.AssignedToId,
                    ParentTaskId = taskVM.ParentTaskId,
                    UpdatedById = userId,
                };
                await _taskRepository.UpdateAsync(task);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await _userManager.Users.ToListAsync();
            ViewBag.Projects = await _projectRepository.GetAllAsyncWithoutInclude();
            return View(taskVM);
        }
        public async Task<JsonResult> GetTaskByProject(int id)
        {
            var tasks = await _taskRepository.GetAllTasksPerProject(id);
            List<TaskViewModel> taskViewModels = new List<TaskViewModel>();
            foreach (var task in tasks)
            {
                TaskViewModel taskVM = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title, 
                };
                taskViewModels.Add(taskVM);
            }
            return Json(taskViewModels);
        }
    }
}
