using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels.ProjectVM;
using Task = SmartTask.Core.Models.Task;

namespace SmartTask.Web.Controllers
{
    public class ProjectProgressController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectProgressController(IProjectService projectService, UserManager<ApplicationUser> userManager)
        {
            _projectService = projectService;
            _userManager = userManager;
        }


        //مسئول عن عرض المشاريع الخاصة ب مستخدم معين 
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var userProjects = await _projectService.GetUserProjectsAsync(currentUserId);

            var projectProgressViewModels = new List<ProjectProgressViewModel>();


            foreach (var project in userProjects)
            {
                // Calculate tasks status
                var tasks = project.Tasks ?? new List<Task>();
                int totalTasks = tasks?.Count ?? 0;
                int completedTasks = tasks.Count(t => new[] { "Completed", "Done" }.Contains(t.Status));
                int inProgressTasks = tasks.Count(t => t.Status == "In Progress");
                int todoTasks = tasks.Count(t => new[] { "Todo", "Not Started" }.Contains(t.Status));


                // Calculate progress percentage
                int progressPercentage = totalTasks > 0 ? (completedTasks * 100) / totalTasks : 0;


                // Calculate days left
                int daysLeft = 0;
                if (project.EndDate.HasValue)
                {
                    daysLeft = Math.Max(0, (project.EndDate.Value - DateTime.Today).Days);
                }

                // Get project members count
                int membersCount = project.ProjectMembers?.Count ?? 0;
                if (!project.ProjectMembers.Any(pm => pm.UserId == project.OwnerId))
                {
                    membersCount++;
                }

                projectProgressViewModels.Add(new ProjectProgressViewModel
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    ProgressPercentage = progressPercentage,
                    DaysLeft = daysLeft,
                    MembersCount = membersCount,
                    CompletedTasks = completedTasks,
                    InProgressTasks = inProgressTasks,
                    TodoTasks = todoTasks,
                    OwnerId = project.OwnerId

                });
            }

            var model = new ProjectsProgressViewModel
            {
                Projects = projectProgressViewModels
            };

            return View(model);
        }



        //مسئول عن عرض تفاصيل المشروع
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var project = await _projectService.GetProjectDetailsAsync(id, currentUserId);




            if (project == null)
            {
                return NotFound();
            }

            int totalTasks = project.Tasks?.Count ?? 0;
            int completedTasks = project.Tasks.Count(t => t.Status == "Completed" || t.Status == "Done");
            int inProgressTasks = project.Tasks.Count(t => t.Status == "In Progress");
            int todoTasks = project.Tasks.Count(t => t.Status == "Todo" || t.Status == "Not Started");

            // Calculate progress percentage
            int progressPercentage = totalTasks > 0 ? (completedTasks * 100) / totalTasks : 0;

            // Calculate days left
            int daysLeft = 0;
            if (project.EndDate.HasValue)
            {
                daysLeft = Math.Max(0, (project.EndDate.Value - DateTime.Today).Days);
            }

            // Get project members
            var members = project.ProjectMembers.Select(pm => pm.User).ToList();
            if (project.Owner != null && !project.ProjectMembers.Any(pm => pm.UserId == project.Owner.Id))
            {
                members.Add(project.Owner);
            }


            var model = new ProjectProgressDetailsViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ProgressPercentage = progressPercentage,
                DaysLeft = daysLeft,
                Members = members,
                CompletedTasks = completedTasks,
                InProgressTasks = inProgressTasks,
                TodoTasks = todoTasks,
                Tasks = project.Tasks.ToList(),
                OwnerId = project.OwnerId,
                Owner = project.Owner

            };

            return View(model);
        }



    }
}
