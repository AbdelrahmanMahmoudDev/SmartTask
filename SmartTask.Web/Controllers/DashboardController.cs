using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.BL.Services;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Web.ViewModels.DashboardVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SmartTask.BL.Services;
using SmartTask.Core.Models.BasePermission;
using System.Text.Json;
using Task = SmartTask.Core.Models.Task;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models.Enums;
using StackExchange.Redis;


namespace SmartTask.Web.Controllers
{
    [Authorize] // Requires users to be logged in
    public class DashboardController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDepartmentService _departmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITaskService _taskService;
        private readonly ITaskRepository _taskRepository;
        private readonly SmartTaskContext _context;
        private const int PageSize = 10;

       

        public DashboardController(
           IProjectService projectService,
            IDashboardService dashboardService,
           UserManager<ApplicationUser> userManager,
           RoleManager<ApplicationRole> roleManager,
           IDepartmentService departmentService,
           IHttpContextAccessor httpContextAccessor,
           ITaskService taskService,
           ITaskRepository taskRepository,
           SmartTaskContext context
           )
        {
            _projectService = projectService;
            _dashboardService = dashboardService;
            _userManager = userManager;
            _roleManager = roleManager;
            _departmentService = departmentService;
            _httpContextAccessor = httpContextAccessor;
            _taskService = taskService;
            _taskRepository = taskRepository;
            _context = context;

        }

        // GET: Dashboard - Main dashboard page with user preferences
        public async Task<IActionResult> Index(string searchString, int? departmentId, int page = 1)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

           
            var userPreference = await _context.UserDashboardPreferences
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.UserId == currentUser.Id);

        
            ViewBag.UserFullName = userPreference?.User?.FullName ?? currentUser.FullName ?? "User";
            ViewBag.LastLoginDate = userPreference?.LastLoginDate ?? DateTime.Now;
            ViewBag.UserPreference = userPreference;

      
            var departments = await _context.Departments.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.SelectedDepartment = departmentId;
            ViewBag.SearchString = searchString;

           
            var allProjects = await _context.Projects.ToListAsync();
            var allTasks = await _context.Tasks.ToListAsync();

        
            var myTasks = await _context.Tasks
                .Where(t => t.Assignments.Any(a => a.UserId == currentUser.Id))
                .ToListAsync();

       
            var viewModel = new DashboardViewModel
            {
            
                TotalProjects = allProjects.Count,
                TotalTasks = allTasks.Count,
                CompletedProjects = allProjects.Count(p => p.Status == "Completed"),
                InProgressProjects = allProjects.Count(p => p.Status == "InProgress"),
                PendingProjects = allProjects.Count(p => p.Status == "Pending"),
                ArchievedProjects = allProjects.Count(p => p.Status == "Archived"),
                CancelledProjects = allProjects.Count(p => p.Status == "Cancelled"),

           
                TodoTasks = myTasks.Count(t => t.Status == Status.Todo),
                InProgressTasks = myTasks.Count(t => t.Status == Status.InProgress),
                CompletedTasks = myTasks.Count(t => t.Status == Status.Done),
                OverdueTasks = myTasks.Count(t => t.EndDate.HasValue &&
                                                 t.EndDate.Value < DateTime.Today &&
                                                 t.Status != Status.Done),
                CancelledTasks = myTasks.Count(t => t.Status == Status.Cancelled),
                ArchievedTasks = myTasks.Count(t => t.Status == Status.Archieved),

                Projects = new List<ProjectViewModel>(),
                MyTasks = myTasks.Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.EndDate,
                    AssignedToName = string.Join(", ", t.Assignments.Select(a => a.User.FullName)),
                    ProjectName = t.Project?.Name,
                    ProjectId = t.ProjectId
                }),
                RecentProjects = new List<ProjectViewModel>()
            };

         
            IQueryable<Project> projectsQuery = _context.Projects
                .Include(p => p.Department)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Assignments)
                        .ThenInclude(a => a.User);

            if (User.IsInRole("Admin"))
            {
                // Admin sees all projects with pagination and filtering
                var projects = await _projectService.GetFilteredProjectsAsync(searchString, departmentId, page, PageSize);
                return View(projects);
            }
            else
            {
                
                projectsQuery = projectsQuery
                    .Where(p => p.Tasks.Any(t => t.Assignments.Any(a => a.UserId == currentUser.Id)));

                // Apply filters
                if (!string.IsNullOrEmpty(searchString))
                {
                    userProjects = userProjects
                        .Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString))
                        .ToList();
                }

                if (departmentId.HasValue)
                {
                    userProjects = userProjects
                        .Where(p => p.DepartmentId == departmentId.Value)
                        .ToList();
                }

            var totalCount = await projectsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var projects = await projectsQuery
                .OrderByDescending(p => p.StartDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

         
            viewModel.Projects = projects.Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Department = new DepartmentViewModel
                {
                    Id = p.Department.Id,
                    Name = p.Department.Name
                },
                Tasks = p.Tasks.Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.EndDate,
                    ProjectId = t.ProjectId,
                    ProjectName = p.Name,
                    AssignedToName = string.Join(", ", t.Assignments.Select(a => a.User.FullName))
                }),
                CompletionPercentage = CalculateCompletionPercentage(p.Tasks)
            });

        
            var recentProjects = await _context.Projects
                .OrderByDescending(p => p.UpdatedAt)
                .Take(5)
                .ToListAsync();

            viewModel.RecentProjects = recentProjects.Select(p => new ProjectViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            });

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            return View(viewModel);
        }

        private double CalculateCompletionPercentage(ICollection<Task> tasks)
        {
            if (tasks == null || tasks.Count == 0)
                return 0;

            var completedTasks = tasks.Count(t => t.Status == Status.Done);
            return (completedTasks * 100.0) / tasks.Count;
        }

        // GET: Dashboard/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            Project project;

            if (User.IsInRole("Admin"))
            {
                // Admin can see any project
                project = await _projectService.GetProjectByIdAsync(id);
            }
            else
            {
                // Regular users and Project Managers need to be associated with the project
                project = await _projectService.GetProjectDetailsAsync(id, currentUser.Id);
            }

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Dashboard/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Only Admin can create new projects
            return View();
        }

        // POST: Dashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,Status,DepartmentId,BranchId,OwnerId")] Project project)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Set creation metadata
            project.CreatedById = currentUser.Id;
            project.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                await _projectService.AddProjectAsync(project);

                // Add owner as project member if owner is specified
                if (!string.IsNullOrEmpty(project.OwnerId))
                {
                    await _projectService.AddMemberAsync(project.Id, project.OwnerId);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Dashboard/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can edit
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Dashboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerId,Name,Description,StartDate,EndDate,Status,DepartmentId,BranchId")] Project project)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (id != project.Id)
            {
                return NotFound();
            }

            // Get the original project for authorization check
            var originalProject = await _projectService.GetProjectByIdAsync(id);
            if (originalProject == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can update
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (originalProject.OwnerId == currentUser.Id ||
                   originalProject.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Preserve original values for fields we don't want to update
                project.CreatedById = originalProject.CreatedById;
                project.CreatedAt = originalProject.CreatedAt;
                project.UpdatedAt = DateTime.Now;

                // Update the project
                await _projectService.UpdateProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Dashboard/FilterByDepartment
        public async Task<IActionResult> FilterByDepartment(int departmentId, string searchString = "", int page = 1)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get departments for filter dropdown
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", departmentId);
            ViewBag.SelectedDepartment = departmentId;
            ViewBag.SearchString = searchString;

            // Get projects based on authorization
            if (User.IsInRole("Admin"))
            {
                // Admin sees all projects for the department with pagination
                var projects = await _projectService.GetFilteredProjectsAsync(searchString, departmentId, page, PageSize);
                return View("Index", projects);
            }
            else
            {
                // Regular users and Project Managers see their own projects for the department
                var userProjects = await _projectService.GetUserProjectsAsync(currentUser.Id);

                // Filter by department and search string
                userProjects = userProjects
                    .Where(p => p.DepartmentId == departmentId)
                    .ToList();

                if (!string.IsNullOrEmpty(searchString))
                {
                    userProjects = userProjects
                        .Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString))
                        .ToList();
                }

                // Manual pagination for filtered user projects
                var totalCount = userProjects.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
                var paginatedProjects = userProjects
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.HasPreviousPage = page > 1;
                ViewBag.HasNextPage = page < totalPages;

                return View("Index", paginatedProjects);
            }
        }

        // GET: Dashboard/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can delete
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Dashboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can delete
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            await _projectService.DeleteProjectAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Dashboard/AddMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> AddMember(int projectId, string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can add members
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            var success = await _projectService.AddMemberAsync(projectId, userId);
            if (!success)
            {
                return BadRequest("Failed to add member to the project.");
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }


        //***************** Settings Part ****************

        // GET: Dashboard/Settings - User dashboard preferences
        public async Task<IActionResult> Settings()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var session = _httpContextAccessor.HttpContext?.Session;
            var sessionKey = $"dashboard_{currentUser.Id}";

            UserDashboardPreference preference;

            // جرب تجيبها من السيشن الأول
            var sessionData = session?.GetString(sessionKey);

            if (!string.IsNullOrEmpty(sessionData))
            {
                preference = JsonSerializer.Deserialize<UserDashboardPreference>(sessionData);
            }
            else
            {
                // أول مرة يدخل أو مفيش بيانات، جيب من الداتا بيز وخزنها في السيشن
                preference = await _dashboardService.GetUserPreferenceAsync(currentUser.Id);

                if (preference != null)
                {
                    session?.SetString(sessionKey, JsonSerializer.Serialize(preference));
                    preference.User = currentUser;
                }
                else
                {
                    preference = new UserDashboardPreference
                    {
                        UserId = currentUser.Id,
                        CreatedAt = DateTime.Now
                    };
                }
            }

            ViewBag.UserFullName = preference.User?.FullName ?? currentUser.FullName ?? "User";
            ViewBag.LastLoginDate = preference.LastLoginDate ?? DateTime.Now;

            return View(preference);
        }


        // POST: Dashboard/Settings - Update user dashboard preferences
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(UserDashboardPreference model)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                if (currentUser != null)
                {
                    // If model is not valid, get the user preference again 
                    var preference = await _dashboardService.GetUserPreferenceAsync(currentUser.Id);
                    ViewBag.UserFullName = preference.User?.FullName ?? currentUser.FullName ?? "User";
                    ViewBag.LastLoginDate = preference.LastLoginDate ?? DateTime.Now;
                }
                return View(model);
            }

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            model.UserId = currentUser.Id;

            await _dashboardService.SaveUserDashboardSettingsAsync(currentUser.Id, model);

            TempData["SuccessMessage"] = "Dashboard preferences updated successfully!";

            return RedirectToAction("Settings", "Dashboard");
        }

        // Update the visibility of a specific widget on the dashboard
        [HttpPost]
        public async Task<IActionResult> UpdateLayout([FromBody] WidgetUpdateViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var preference = await _dashboardService.GetUserPreferenceAsync(currentUser.Id);

            // According to the widget type, update the visibility
            switch (model.WidgetType.ToLower())
            {
                case "project-status":
                    preference.ShowProjectStatus = model.IsVisible;
                    break;
                case "recent-projects":
                    preference.ShowRecentProjects = model.IsVisible;
                    break;
                case "upcoming-tasks":
                    preference.ShowUpcomingTasks = model.IsVisible;
                    break;
                case "activity-chart":
                    preference.ShowActivityChart = model.IsVisible;
                    break;
            }

            await _dashboardService.UpdateUserPreferenceAsync(preference);

            return Json(new { success = true });
        }

        // Change the preferred dashboard view (grid or list view)
        [HttpPost]
        public async Task<IActionResult> ChangeView([FromBody] ViewUpdateViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.ViewType))
            {
                return Json(new { success = false });
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var preference = await _dashboardService.GetUserPreferenceAsync(currentUser.Id);

            preference.PreferredView = model.ViewType;
            await _dashboardService.UpdateUserPreferenceAsync(preference);

            return Json(new { success = true });
        }

        // Update the number of recent projects displayed on the dashboard
        [HttpPost]
        public async Task<IActionResult> UpdateRecentProjectsCount([FromBody] RecentProjectsCountViewModel model)
        {
            if (model == null || model.Count < 1 || model.Count > 20)
            {
                return Json(new { success = false, message = "Invalid count value" });
            }
        
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var preference = await _dashboardService.GetUserPreferenceAsync(currentUser.Id);
            preference.RecentProjectsCount = model.Count;

            await _dashboardService.UpdateUserPreferenceAsync(preference);

            return Json(new { success = true });
        }
        

        [HttpGet]
        public async Task<IActionResult> MyTasks()
        {
            List<Task> tasks;
            var currentUser = await _userManager.GetUserAsync(User);

            tasks = await _taskService.TasksForUser(currentUser.Id);


            return View(tasks);


        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(Task updatedTask)
        {
            var task = await _taskRepository.GetByIdAsync(updatedTask.Id);
            if (task == null)
                return NotFound();

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.Status = updatedTask.Status;
            task.Priority = updatedTask.Priority;
            task.StartDate = updatedTask.StartDate;
            task.EndDate = updatedTask.EndDate;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            return RedirectToAction("MyTasks");
        }


        public async Task<IActionResult> TaskOverview()
        {
            var tasks = await _taskRepository.GetTasksOverviewAsync();

            ViewBag.TotalTasks = tasks.Count();
            ViewBag.TodoTasks = tasks.Count(t => t.Status == Status.Todo);
            ViewBag.InProgressTasks = tasks.Count(t => t.Status == Status.InProgress);
            ViewBag.CompletedTasks = tasks.Count(t => t.Status == Status.Done);
            ViewBag.ArchievedTasks = tasks.Count(t => t.Status == Status.Archieved);
            ViewBag.CancelledTasks = tasks.Count(t => t.Status == Status.Cancelled); 
            ViewBag.OverdueTasks = tasks.Count(t => t.EndDate < DateTime.Now && t.Status != Status.Done);

            return View(tasks);
        }

    }

}

