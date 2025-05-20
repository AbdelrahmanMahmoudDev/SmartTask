using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.BL.Service.Hubs;
using SmartTask.BL.Services;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.Enums;
using SmartTask.Core.Models.Notification;
using SmartTask.Core.ViewModels;
using SmartTask.DataAccess.Data;
//using SmartTask.Web.Models;
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
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ITaskService _taskService;
        private readonly IWebHostEnvironment _environment;
        private readonly ITaskDependencyRepository _taskDependencyRepo;

        public TaskController(ITaskRepository taskRepository, IProjectRepository projectRepository,
            UserManager<ApplicationUser> usermanager, SmartTaskContext context,
            IAssignTaskRepository assignTaskRepository, INotificationRepository notificationRepository,
            IHubContext<NotificationHub> hub, ITaskService taskService
            , IWebHostEnvironment environment, ITaskDependencyRepository taskDependencyRepo)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _userManager = usermanager;
            _context = context;
            _assignTaskRepository = assignTaskRepository;
            _notificationRepository = notificationRepository;
            _hub = hub;
            _taskService = taskService;
            _environment = environment;
            _taskDependencyRepo = taskDependencyRepo;
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskService.Details(id);
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
            Comment comment = await _taskService.AddComment(taskId, authorId, content);
            if (comment == null)
            {
                return StatusCode(500, "Failed to add comment");
            }

            IEnumerable<AssignTask> taskUsers = await _assignTaskRepository.GetByTaskIdAsync(comment.TaskId);

            //SignalR Part
            Notification notification;
            var user = await _userManager.GetUserAsync(User);
            string NotificationMessage = $"{user.FullName} Commented on : {comment.Task.Title}";
            foreach (var receiverID in taskUsers)
            {
                notification = new Notification
                {
                    Message = NotificationMessage,
                    Type = "Comment",
                    SenderId = user.Id,
                    ReceiverId = receiverID.UserId
                };

                await _hub.Clients.User(receiverID.UserId).SendAsync("assignedtask", notification);
                //save to db
                await _notificationRepository.AddAsync(notification);
            }

            return Json(new
            {
                author = comment.Author?.FullName,
                content = comment.Content,
                createdAt = comment.CreatedAt.ToString("dd-MM-yyyy HH:mm")
            });

            //return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddAttachment(int taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected");
            }

            var user = await _userManager.GetUserAsync(User);

            var attachment = await _taskService.AddAttachment(taskId, file, user.Id, _environment.WebRootPath);

            if (attachment == null)
            {
                return StatusCode(500, "Failed to upload attachment");
            }

            //SignalR Part
            Notification notification;
            IEnumerable<AssignTask> taskUsers = await _assignTaskRepository.GetByTaskIdAsync(attachment.TaskId);
            string NotificationMessage = $"{user.FullName} Added Attachment on : {attachment.Task.Title}";
            foreach (var receiverID in taskUsers)
            {
                notification = new Notification
                {
                    Message = NotificationMessage,
                    Type = "Attachment",
                    SenderId = user.Id,
                    ReceiverId = receiverID.UserId
                };

                await _hub.Clients.User(receiverID.UserId).SendAsync("assignedtask", notification);
                //save to db
                await _notificationRepository.AddAsync(notification);
            }

            return Json(new
            {
                fileName = attachment.FileName,
                filePath = attachment.FilePath,
                createdAt = attachment.CreatedAt.ToString("dd-MM-yyyy HH:mm")
            });
        }

        public async Task<IActionResult> TasksForUserInProject(int projectId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _taskService.TasksForUserInProject(projectId, userId);
            return View("Tasks", tasks);
        }

        public async Task<IActionResult> TasksForProject(int projectId)
        {
            var tasks = await _taskService.TasksForProject(projectId);
            return View("Tasks", tasks);
        }

        public async Task<IActionResult> TasksForUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _taskService.TasksForUser(userId);
            return View("Tasks", tasks);
        }

        [HttpGet]
        public async Task<int> Depend(int taskid)
        {
            return await _taskService.NumofDepend(taskid);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int taskid)
        {
            var task = await _taskService.GetTask(taskid);
            if (task.Status != Core.Models.Enums.Status.Todo)
            {
                return BadRequest("Task can't be deleted because it has started.");
            }
            var task1 = await _taskService.ISAParent(taskid);
            if (task1)
            {
                return BadRequest("Task can't be deleted because this task has a childern.");
            }

            //SignalR Part
            Notification notification;

            var assignedUsers = await _assignTaskRepository.GetByTaskIdAsync(taskid);

            var user = await _userManager.GetUserAsync(User);
            string NotificationMessage = $"{user.FullName} Deleted Task : {task.Title}";
            foreach (var receiverID in assignedUsers)
            {

                notification = new Notification
                {
                    Message = NotificationMessage,
                    Type = "Delete",
                    SenderId = user.Id,
                    ReceiverId = receiverID.UserId
                };

                await _hub.Clients.User(receiverID.UserId).SendAsync("assignedtask", notification);
                //save to db
                await _notificationRepository.AddAsync(notification);
            }

            //Delete Task
            await _taskService.DeleteDepend(taskid);
            await _taskService.Delete(taskid);
            return Ok();
        }

        public async Task<IActionResult> Loadnodes(int id)
        {
            var taskViewDeps = (await _taskService.Loadnodes(id)).Select(n =>
            {
                return new TaskDendenciesViewModel
                {
                    Id = n.TaskId,
                    Name = n.Name,
                    IsSelected = n.IsSelected,
                    DependencyType = n.DependencyType
                };
            }).ToList();
            return PartialView("_TaskDend", taskViewDeps);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSelectedTasks(int SelectedTaskId, List<int> selectedTaskIds, List<DependencyType> dependencyTypes)
        {
            await _taskService.SaveSelectedTasks(SelectedTaskId, selectedTaskIds,dependencyTypes);
            return Ok();
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
            // Notification for signalR
            Notification notification;
            string NotificationMessage = "NA";
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
            //task.Assignments = await _assignTaskRepository.FindTasksAssignedToUserByIds(taskVM.AssignedToId);

            //SignalR Part

            var user = await _userManager.GetUserAsync(User);
            NotificationMessage = $"{user.FullName} Assigned new Task : {taskVM.Title}";
            foreach (var receiverID in taskVM.AssignedToId)
            {
                notification = new Notification
                {
                    Message = NotificationMessage,
                    Type = "NewTask",
                    SenderId = userId,
                    ReceiverId = receiverID
                };

                await _hub.Clients.User(receiverID).SendAsync("assignedtask", notification);
                //save to db
                //_context.ChangeTracker.Clear();
                await _notificationRepository.AddAsync(notification);
            }
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
                    await _assignTaskRepository.ModifyTasksToUserByIds(userId, _task, taskVM.AssignedToId);
                    _task.Title = taskVM.Title;
                    _task.Description = taskVM.Description;
                    _task.StartDate = taskVM.StartDate;
                    _task.EndDate = taskVM.EndDate;
                    _task.Status = taskVM.Status;
                    _task.Priority = taskVM.Priority;
                    _task.Status = taskVM.Status;
                    _task.Priority = taskVM.Priority;
                    _task.ParentTaskId = taskVM.ParentTaskId;
                    await _taskRepository.UpdateAsync(_task);

                    //SignalR Part
                    Notification notification;
                    string NotificationMessage = "NA";
                    var user = await _userManager.GetUserAsync(User);
                    NotificationMessage = $"{user.FullName} Updated Assigned Task : {taskVM.Title}";
                    foreach (var receiverID in taskVM.AssignedToId)
                    {
                        notification = new Notification
                        {
                            Message = NotificationMessage,
                            Type = "UpdateTask",
                            SenderId = userId,
                            ReceiverId = receiverID
                        };

                        await _hub.Clients.User(receiverID).SendAsync("assignedtask", notification);

                        //save to db
                        await _notificationRepository.AddAsync(notification);
                    }
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
                .Include(t => t.Assignments).Include(t => t.Project)
                //.ThenInclude(a=>a.Branch).ThenInclude(a => a.Department)
                .AsQueryable();


            if (filter.Status != 0)
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
        public async Task<IActionResult> IncreaseStatus(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            var sucsessortasks = await _taskDependencyRepo.GetBySuccessorIdAsync(id);
            if (task == null)
            {
                return NotFound(new { message = "Task Not found" });
            }
            foreach (var sucsessortask in sucsessortasks)
            {

                if (sucsessortask.DependencyType==DependencyType.FinishToStart && sucsessortask.Predecessor.Status!=Status.Done)
                {
                    return BadRequest(new { message = "This task depends on another task. You must finish it first." });
                }
            }
            if (task.Status == Status.Done)
            {
                return BadRequest(new { message = "Task is already completed." });
            }
            ++task.Status;
            await _taskRepository.UpdateAsync(task);

            return PartialView("PartialViews/_Status", task);
        }
    }
}