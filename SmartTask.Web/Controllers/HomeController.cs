using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Data;
using SmartTask.Web.ViewModels;
using SmartTask.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using SmartTask.Core.Models.Enums;
using System.Security.Claims;

namespace SmartTask.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationService notificationService;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;

        public HomeController(
            ILogger<HomeController> logger,
            INotificationService notificationService,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository)
        {
            _logger = logger;
            this.notificationService = notificationService;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
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

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns the current user's project dashboard with task statistics and details
        /// </summary>
        [Authorize]
        public async Task<IActionResult> UserProjects()
        {
            try
            {
                // Get current user ID from the authentication context
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Initialize view model
                var viewModel = new UserProjectsViewModel();

                // Get all projects where the current user is a member
                var userProjects = await _projectRepository.GetUserProjectsAsync(currentUserId);

                // Filter projects to exclude archived and completed ones
                var activeProjects = userProjects
                    .Where(p => !string.Equals(p.Status, "Archived", StringComparison.OrdinalIgnoreCase) &&
                               !string.Equals(p.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var project in activeProjects)
                {
                    // Get all tasks for this project that are assigned to the current user
                    var userTasksInProject = await _taskRepository.GetByProjectIdAsync(project.Id);
                    var assignedTasks = userTasksInProject
                        .Where(t => t.Assignments.Any(a => a.UserId == currentUserId))
                        .ToList();

                    // Calculate task statistics for the user
                    var totalTasks = assignedTasks.Count;
                    var completedTasks = assignedTasks.Count(t => t.Status == Status.Done);
                    var inProgressTasks = assignedTasks.Where(t => t.Status == Status.InProgress).ToList();
                    var upcomingTasks = assignedTasks.Where(t => t.Status == Status.Todo).ToList();

                    // Create project summary view model
                    var projectSummary = new ProjectSummaryViewModel
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        StartDate = project.StartDate,
                        EndDate = project.EndDate,
                        Description = project.Description,
                        TotalTasksAssignedToUser = totalTasks,
                        CompletedTasksByUser = completedTasks,
                        InProgressTasksByUser = inProgressTasks.Count
                    };

                    // Map in-progress tasks
                    foreach (var task in inProgressTasks)
                    {
                        var inProgressTaskViewModel = new InProgressTaskViewModel
                        {
                            TaskId = task.Id,
                            TaskName = task.Title,
                            Description = task.Description,
                            EndDate = task.EndDate,
                            Priority = task.Priority,
                            IsDelayed = IsTaskDelayed(task.EndDate, project.EndDate)
                        };

                        projectSummary.InProgressTasks.Add(inProgressTaskViewModel);
                    }

                    // Map upcoming tasks
                    foreach (var task in upcomingTasks)
                    {
                        var upcomingTaskViewModel = new UpcomingTaskViewModel
                        {
                            TaskId = task.Id,
                            TaskName = task.Title,
                            Description = task.Description,
                            StartDate = task.StartDate,
                            EndDate = task.EndDate
                        };

                        projectSummary.UpcomingTasks.Add(upcomingTaskViewModel);
                    }

                    viewModel.Projects.Add(projectSummary);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user projects for user {UserId}",
                    User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Return empty view model in case of error
                return View(new UserProjectsViewModel());
            }
        }

        /// <summary>
        /// Determines if a task is delayed based on its end date compared to the project end date
        /// </summary>
        /// <param name="taskEndDate">The task's end date</param>
        /// <param name="projectEndDate">The project's end date</param>
        /// <returns>True if the task end date exceeds the project end date</returns>
        private bool IsTaskDelayed(DateTime? taskEndDate, DateTime? projectEndDate)
        {
            if (!taskEndDate.HasValue || !projectEndDate.HasValue)
            {
                return false;
            }

            return taskEndDate.Value > projectEndDate.Value;
        }
    }
}