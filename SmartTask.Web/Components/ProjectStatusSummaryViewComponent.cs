using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.ObjectModelRemoting;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels.DashboardVM;
using System.Security.Claims;

namespace SmartTask.Web.Components
{
    public class ProjectStatusSummaryViewComponent : ViewComponent
    {
        private readonly IProjectService _projectService;

        public ProjectStatusSummaryViewComponent(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId = null)
        {
            userId ??= ViewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            List<Project> projects;
            if (string.IsNullOrEmpty(userId))
            {
                projects = await _projectService.GetFilteredProjectsAsync(null, null, 1, int.MaxValue);
            }
            else
            {
                projects = await _projectService.GetUserProjectsAsync(userId);
            }

            int totalProjects = projects.Count;
            int totalTasks = projects.Sum(p => p.Tasks?.Count ?? 0);

            var viewModel = new ProjectStatusSummaryViewModel
            {
                TotalProjects = totalProjects,
                PendingProjects = projects.Count(p => p.Status?.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true),
                InProgressProjects = projects.Count(p => p.Status?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true),
                CompletedProjects = projects.Count(p => p.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true),
                OverdueProjects = projects.Count(p => p.EndDate.HasValue &&
                                              p.EndDate.Value < DateTime.Now &&
                                              !p.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true),
                TotalTasks = totalTasks,
                CompletionPercentage = totalProjects > 0 ? (int)((double)projects.Count(p => p.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true) / totalProjects * 100) : 0,
                InProgressPercentage = totalProjects > 0 ? (int)((double)projects.Count(p => p.Status?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true) / totalProjects * 100) : 0,
                PendingPercentage = totalProjects > 0 ? (int)((double)projects.Count(p => p.Status?.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true) / totalProjects * 100) : 0,
                TaskPerProject = totalProjects > 0 ? (int)((double)totalTasks / totalProjects) : 0
            };

            return View(viewModel);
        }
    }
}
