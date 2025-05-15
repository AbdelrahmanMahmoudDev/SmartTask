using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartTask.Api.DTOs;
using SmartTask.BL.Service.Hubs;
using SmartTask.BL.Services;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.Notification;
using SmartTask.Core.ViewModels;
using SmartTask.DataAccess.Data;
using SmartTask.Web.ViewModels;
using System.Security.Claims;
using TaskModel = SmartTask.Core.Models.Task;

namespace SmartTask.Web.ApiControllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly SmartTaskContext _context;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAssignTaskRepository _assignTaskRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly TaskService _taskService;
        private readonly IWebHostEnvironment _environment;

        public TaskController(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            UserManager<ApplicationUser> usermanager,
            SmartTaskContext context,
            IAssignTaskRepository assignTaskRepository,
            INotificationRepository notificationRepository,
            IHubContext<NotificationHub> hub,
            TaskService taskService,
            IWebHostEnvironment environment)
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
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskService.Details(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Comment content is required");
            }

            var comment = await _taskService.AddComment(request.TaskId, request.AuthorId, request.Content);
            if (comment == null)
            {
                return StatusCode(500, "Failed to add comment");
            }

            var taskUsers = await _assignTaskRepository.GetByTaskIdAsync(comment.TaskId);
            var user = await _userManager.GetUserAsync(User);
            string notificationMessage = $"{user.FullName} Commented on : {comment.Task.Title}";

            foreach (var receiverID in taskUsers)
            {
                var notification = new Notification
                {
                    Message = notificationMessage,
                    Type = "Comment",
                    SenderId = user.Id,
                    ReceiverId = receiverID.UserId
                };

                await _hub.Clients.User(receiverID.UserId).SendAsync("assignedtask", notification);
                await _notificationRepository.AddAsync(notification);
            }

            return Ok(new
            {
                author = comment.Author?.FullName,
                content = comment.Content,
                createdAt = comment.CreatedAt.ToString("dd-MM-yyyy HH:mm")
            });
        }

        [HttpPost("AddAttachment")]
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

            var taskUsers = await _assignTaskRepository.GetByTaskIdAsync(attachment.TaskId);
            string notificationMessage = $"{user.FullName} Added Attachment on : {attachment.Task.Title}";

            foreach (var receiverID in taskUsers)
            {
                var notification = new Notification
                {
                    Message = notificationMessage,
                    Type = "Attachment",
                    SenderId = user.Id,
                    ReceiverId = receiverID.UserId
                };

                await _hub.Clients.User(receiverID.UserId).SendAsync("assignedtask", notification);
                await _notificationRepository.AddAsync(notification);
            }

            return Ok(new
            {
                fileName = attachment.FileName,
                filePath = attachment.FilePath,
                createdAt = attachment.CreatedAt.ToString("dd-MM-yyyy HH:mm")
            });
        }

        [HttpGet("TasksForUserInProject/{projectId}")]
        public async Task<IActionResult> TasksForUserInProject(int projectId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _taskService.TasksForUserInProject(projectId, userId);
            return Ok(tasks);
        }

        [HttpGet("TasksForProject/{projectId}")]
        public async Task<IActionResult> TasksForProject(int projectId)
        {
            var tasks = await _taskService.TasksForProject(projectId);
            return Ok(tasks);
        }

        [HttpGet("TasksForUser")]
        public async Task<IActionResult> TasksForUser()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = _taskService.TasksForUser(userId);
            return Ok(tasks);
        }

        [HttpGet("Depend/{taskId}")]
        public async Task<IActionResult> Depend(int taskId)
        {
            int numDepends = await _taskService.NumofDepend(taskId);
            return Ok(numDepends);
        }

        [HttpDelete("DeleteTask/{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var task = await _taskService.GetTask(taskId);
            if (task.Status != Core.Models.Enums.Status.Todo)
            {
                return BadRequest("Task can't be deleted because it has started");
            }

            var assignedUsers = await _assignTaskRepository.GetByTaskIdAsync(taskId);
            var user = await _userManager.GetUserAsync(User);
            string notificationMessage = $"{user.FullName} Deleted Task : {task.Title}";

            foreach (var receiverID in assignedUsers)
            {
                var notification = new Notification
                {
                    Message = notificationMessage,
                    Type = "Delete",
                    SenderId = user.Id,
                    ReceiverId = receiverID.UserId
                };

                await _hub.Clients.User(receiverID.UserId).SendAsync("assignedtask", notification);
                await _notificationRepository.AddAsync(notification);
            }

            await _taskService.DeleteDepend(taskId);
            await _taskService.Delete(taskId);

            return Ok(new { message = "Task deleted successfully" });
        }

        [HttpGet("LoadNodes/{id}")]
        public async Task<IActionResult> LoadNodes(int id)
        {
            var taskDeps = (await _taskService.Loadnodes(id)).Select(n => new TaskDendenciesViewModel
            {
                Id = n.TaskId,
                Name = n.Name,
                IsSelected = n.IsSelected
            }).ToList();

            return Ok(taskDeps);
        }

        [HttpPost("SaveSelectedTasks")]
        public async Task<IActionResult> SaveSelectedTasks([FromBody] SaveSelectedTasksDTO request)
        {
            await _taskService.SaveSelectedTasks(request.SelectedTaskId, request.SelectedTaskIds);
            return Ok(new { message = "Tasks saved successfully" });
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var tasks = await _taskRepository.GetAllAsync();

            var taskViewModels = tasks.Select(task =>
            {
                var taskVM = new TaskViewModel
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
                    ParentTaskId = task.ParentTaskId,
                    UpdatedById = task.UpdatedById,
                    ProjectName = task.Project.Name
                };

                foreach (var assignment in task.Assignments)
                {
                    taskVM.AssignedToId.Add(assignment.UserId);
                }

                return taskVM;
            }).ToList();

            return Ok(taskViewModels);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TaskViewModel taskVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            taskVM.CreatedById = userId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TaskModel task = new TaskModel
            {
                Title = taskVM.Title,
                Description = taskVM.Description,
                StartDate = taskVM.StartDate,
                EndDate = taskVM.EndDate,
                Status = taskVM.Status,
                Priority = taskVM.Priority,
                CreatedById = taskVM.CreatedById,
                CreatedAt = DateTime.Now,
                ProjectId = taskVM.ProjectId,
                ParentTaskId = taskVM.ParentTaskId
            };

            await _taskRepository.AddAsync(task);

            foreach (var assignedUserId in taskVM.AssignedToId)
            {
                await _assignTaskRepository.AddAsync(new AssignTask
                {
                    TaskId = task.Id,
                    UserId = assignedUserId
                });

                var notification = new Notification
                {
                    Message = $"{User.Identity.Name} assigned task: {task.Title}",
                    Type = "Assign",
                    SenderId = userId,
                    ReceiverId = assignedUserId
                };
                await _hub.Clients.User(assignedUserId).SendAsync("assignedtask", notification);
                await _notificationRepository.AddAsync(notification);
            }

            return Ok(new { message = "Task created successfully", taskId = task.Id });
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] TaskViewModel taskVM)
        {
            var existingTask = await _taskRepository.GetByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.Title = taskVM.Title;
            existingTask.Description = taskVM.Description;
            existingTask.StartDate = taskVM.StartDate;
            existingTask.EndDate = taskVM.EndDate;
            existingTask.Status = taskVM.Status;
            existingTask.Priority = taskVM.Priority;
            existingTask.ProjectId = taskVM.ProjectId;
            existingTask.ParentTaskId = taskVM.ParentTaskId;
            existingTask.UpdatedAt = DateTime.Now;
            existingTask.UpdatedById = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _taskRepository.UpdateAsync(existingTask);

            var currentAssignments = await _assignTaskRepository.GetByTaskIdAsync(id);
            foreach (var assign in currentAssignments)
            {
                await _assignTaskRepository.DeleteAsync(assign.TaskId, assign.UserId);
            }

            foreach (var userId in taskVM.AssignedToId)
            {
                await _assignTaskRepository.AddAsync(new AssignTask
                {
                    TaskId = id,
                    UserId = userId
                });
            }

            return Ok(new { message = "Task updated successfully" });
        }
    }
}