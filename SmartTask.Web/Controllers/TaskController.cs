using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.ViewModels;
using SmartTask.DataAccess.Data;
using SmartTask.Web.ViewModels;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using TaskModel = SmartTask.Core.Models.Task;

namespace SmartTask.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly SmartTaskContext _context;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAssignTaskRepository _assignTaskRepository;

        public TaskController(ITaskRepository taskRepository, IProjectRepository projectRepository,
            UserManager<ApplicationUser> usermanager, SmartTaskContext context, IAssignTaskRepository assignTaskRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userManager = usermanager;
            _context = context;
            _assignTaskRepository = assignTaskRepository;
        }

        public IActionResult Details(int id)
        {
            var task = _context.Tasks.Include(t => t.Comments).ThenInclude(c => c.Author).Include(t => t.Attachments).FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            return PartialView("_DetailsPartial", task);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int taskId, string authorId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Comment content is required");
            }

            var comment = new Comment
            {
                TaskId = taskId,
                AuthorId = authorId,
                Content = content.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Ok();
            return RedirectToAction("Details", new { id = taskId });
        }

        [HttpPost]
        public async Task<IActionResult> AddAttachment(int taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                TaskId = taskId,
                FileName = file.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                UploadedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return Ok();
            return RedirectToAction("Details", new { id = taskId });
        }

        public async Task<IActionResult> TasksForUserInProject(int projectId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _context.Tasks.Include(t => t.Assignments).Where(t => t.ProjectId == projectId && t.Assignments.Select(u => u.UserId == userId).FirstOrDefault()).ToListAsync();
            return View("Tasks", tasks);
        }

        public async Task<IActionResult> TasksForProject(int projectId)
        {
            var tasks = await _context.Tasks.Where(x=>x.ProjectId == projectId).ToListAsync();
            return View("Tasks", tasks);
        }

        public async Task<IActionResult> TasksForUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _context.Tasks.Include(T => T.Assignments).Where(t => t.Assignments.Select(a => a.UserId == userId).FirstOrDefault()).ToListAsync();
            return View("Tasks", tasks);
        }

        [HttpGet]
        public int Depend(int taskid)
        {
            return _context.TaskDependencies
                .Count(t => t.PredecessorId == taskid);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int taskid)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == taskid);
            if (task.Status != Core.Models.Enums.Status.Todo)
            {
                return BadRequest("Task can't be deleted because it has started.");
            }

            var dependencies = await _context.TaskDependencies.Where(t => t.PredecessorId == taskid || t.SuccessorId == taskid).ToListAsync();

            _context.TaskDependencies.RemoveRange(dependencies);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> Loadnodes(int id)
        {
            var graph = new Dictionary<int, List<int>>();
            var visited = new HashSet<int>();
            var task=_context.Tasks.FirstOrDefault(x=>x.Id==id);
            var allTasks = _context.Tasks.Where(x=>x.ProjectId==task.ProjectId);

            await _context.TaskDependencies.ForEachAsync(t =>
            {
                if (!graph.ContainsKey(t.PredecessorId))
                {
                    graph[t.PredecessorId] = new List<int>();
                }
                graph[t.PredecessorId].Add(t.SuccessorId);
            });

            DFS(id, graph, visited);

            var notReachable = allTasks.ToList().ExceptBy(visited, e => e.Id);
            var existingDeps = _context.TaskDependencies.Where(x => x.SuccessorId == id).Select(e => e.PredecessorId).ToList();

            var taskViewDeps = notReachable.Select(n => new TaskDendenciesViewModel
            {
                Id = n.Id,
                Name = n.Title,
                IsSelected = existingDeps.Contains(n.Id)
            }).ToList();

            return PartialView("_TaskDend", taskViewDeps);
        }

        private static void DFS(int node, Dictionary<int, List<int>> graph, HashSet<int> visited)
        {
            if (!visited.Add(node)) return;

            if (graph.TryGetValue(node, out var neighbors))
            {
                foreach (var neighbor in neighbors)
                {
                    DFS(neighbor, graph, visited);
                }
            }
        }

        [HttpPost]
        public IActionResult SaveSelectedTasks(int SelectedTaskId, List<int> selectedTaskIds)
        {
            var existingDependencies = _context.TaskDependencies.Where(td => td.SuccessorId == SelectedTaskId).ToList();

            foreach (var selectedId in selectedTaskIds)
            {
                if (!existingDependencies.Any(td => td.PredecessorId == selectedId))
                {
                    _context.TaskDependencies.Add(new TaskDependency
                    {
                        SuccessorId = SelectedTaskId,
                        PredecessorId = selectedId
                    });
                }

            }

            foreach (var dependency in existingDependencies)
            {
                if (!selectedTaskIds.Contains(dependency.PredecessorId))
                {
                    _context.TaskDependencies.Remove(dependency);
                }
            }

            _context.SaveChanges();
            return Json(new { success = true, selected = selectedTaskIds });
        }

        public async Task<IActionResult> Index()
        {
            List<TaskViewModel> taskViewModels = new List<TaskViewModel>();
            var tasks = await _taskRepository.GetAllAsync();
            foreach (var task in tasks)
            {
                TaskViewModel taskVM = new TaskViewModel();

                taskVM.Id = task.Id;
                taskVM.Title = task.Title;
                taskVM.Description = task.Description;
                taskVM.StartDate = task.StartDate;
                taskVM.EndDate = task.EndDate;
                taskVM.Status = task.Status;
                taskVM.Priority = task.Priority;
                taskVM.CreatedById = task.CreatedById;
                taskVM.CreatedAt = task.CreatedAt;
                taskVM.UpdatedAt = task.UpdatedAt;
                taskVM.ProjectId = task.ProjectId;
                foreach (var assignment in task.Assignments)
                {
                    taskVM.AssignedToId.Add(assignment.UserId);
                }
                taskVM.ParentTaskId = task.ParentTaskId;
                taskVM.UpdatedById = task.UpdatedById;
                taskVM.ProjectName = task.Project.Name;

                taskViewModels.Add(taskVM);
            }
            ViewBag.Users = await _userManager.Users.ToListAsync();
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
                CreatedById = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ProjectId = taskVM.ProjectId,
                ParentTaskId = taskVM.ParentTaskId,
                UpdatedById = userId,
            };
            await _taskRepository.AddAsync(task);
            await _assignTaskRepository.AssignTasksToUserByIds(taskVM.AssignedToId, task, User);
            task.Assignments = await _assignTaskRepository.FindTasksAssignedToUserByIds(taskVM.AssignedToId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TaskViewModel taskVM = new TaskViewModel();

            taskVM.Id = task.Id;
            taskVM.Title = task.Title;
            taskVM.Description = task.Description;
            taskVM.StartDate = task.StartDate;
            taskVM.EndDate = task.EndDate;
            taskVM.Status = task.Status;
            taskVM.Priority = task.Priority;
            taskVM.CreatedById = task.CreatedById;
            taskVM.CreatedAt = task.CreatedAt;
            taskVM.UpdatedAt = task.UpdatedAt;
            taskVM.ProjectId = task.ProjectId;
            foreach (var assignment in task.Assignments)
            {
                taskVM.AssignedToId.Add(assignment.UserId);
            }
            taskVM.ParentTaskId = task.ParentTaskId;
            taskVM.UpdatedById = task.UpdatedById;
            taskVM.ProjectName = task.Project.Name;


            var tasks = await GetTaskByProject(task.ProjectId);
            ViewBag.Tasks = JsonSerializer.Deserialize<List<TaskViewModel>>(JsonSerializer.Serialize(tasks.Value));
            ViewBag.Users = await _userManager.Users.ToListAsync();
            ViewBag.Projects = await _projectRepository.GetAllAsyncWithoutInclude();
            return View(taskVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaskViewModel taskVM, [FromRoute] int id)
        {
            if (taskVM.Id == id)
            {
                if (ModelState.IsValid)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var _task = await _taskRepository.GetByIdAsync(taskVM.Id);
                    _task.Title = taskVM.Title;
                    _task.Description = taskVM.Description;
                    _task.StartDate = taskVM.StartDate;
                    _task.EndDate = taskVM.EndDate;
                    _task.Status = taskVM.Status;
                    _task.Priority = taskVM.Priority;
                    _task.Status = taskVM.Status;
                    _task.Priority = taskVM.Priority;
                    _task.ParentTaskId = taskVM.ParentTaskId;
                    await _assignTaskRepository.ModifyTasksToUserByIds(userId, _task, taskVM.AssignedToId);
                    await _taskRepository.UpdateAsync(_task);
                    return RedirectToAction(nameof(Index));
                }
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
        public async Task<JsonResult> ValidateStartDate(DateTime StartDate, int ProjectId)
        {
            var project = await _projectRepository.GetByIdAsync(ProjectId);

            if (StartDate < project.StartDate || StartDate > project.EndDate)
            {
                return Json(false);
            }

            return Json(true);
        }

        public async Task<JsonResult> ValidateEndDate(DateTime EndDate, DateTime StartDate, int ProjectId)
        {
            var project = await _projectRepository.GetByIdAsync(ProjectId);

            if (EndDate < project.StartDate || EndDate > project.EndDate || EndDate < StartDate)
            {
                return Json(false);
            }

            return Json(true);
        }
        public async Task<IActionResult> Filter(TaskFilterViewModel filter)
        {
            var query = _context.Tasks
                .Include(t => t.Assignments).Include(t=>t.Project)
                //.ThenInclude(a=>a.Branch).ThenInclude(a => a.Department)
                .AsQueryable();

           
                query = query.Where(t => t.Status == filter.Status);

            if (filter.StartDate.HasValue)
                query = query.Where(t => t.StartDate == filter.StartDate);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.EndDate == filter.EndDate);

            if (!string.IsNullOrEmpty(filter.AssignedToUserId))
                query = query.Where(t => t.Assignments.Any(a => a.UserId == filter.AssignedToUserId));

            //if (filter.DepartmentId.HasValue)
            //    query = query.Where(t => t.Assignments.Any(a=>a.Department.Id == filter.DepartmentId);

            //if (filter.BranchId.HasValue)
            //    query = query.Where(t => t.Assignments.Any(a=>a.Branch.Id== filter.BranchId);

            var result = await query.ToListAsync();
            var taskViewModels = new List<TaskViewModel>();
            foreach (var task in result)
            {
                TaskViewModel taskVM = new TaskViewModel();

                taskVM.Id = task.Id;
                taskVM.Title = task.Title;
                taskVM.Description = task.Description;
                taskVM.StartDate = task.StartDate;
                taskVM.EndDate = task.EndDate;
                taskVM.Status = task.Status;
                taskVM.Priority = task.Priority;
                taskVM.CreatedById = task.CreatedById;
                taskVM.CreatedAt = task.CreatedAt;
                taskVM.UpdatedAt = task.UpdatedAt;
                taskVM.ProjectId = task.ProjectId;
                foreach (var assignment in task.Assignments)
                {
                    taskVM.AssignedToId.Add(assignment.UserId);
                }
                taskVM.ParentTaskId = task.ParentTaskId;
                taskVM.UpdatedById = task.UpdatedById;
                taskVM.ProjectName = task.Project.Name;

                taskViewModels.Add(taskVM);
            }

            return PartialView("PartialViews/_TaskTable", taskViewModels);
        }
    }
}