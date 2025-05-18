using Microsoft.AspNetCore.Mvc;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels.DashboardVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTask.Web.Components
{
    public class RecentProjectsViewComponent : ViewComponent
    {
        private readonly IProjectService _projectService;

        public RecentProjectsViewComponent(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId = null, int count = 5)
        {
            // Get the user ID from claims if not provided
            userId ??= ViewContext.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Get the list of projects for the user
            List<Project> projects;
            if (string.IsNullOrEmpty(userId))
            {
                projects = await _projectService.GetFilteredProjectsAsync(null, null, 1, count);
            }
            else
            {
                projects = await _projectService.GetUserProjectsAsync(userId);
            }

            // Order by created date and take the specified number
            var projectsList = projects.OrderByDescending(p => p.CreatedAt)
                                     .Take(count).ToList();

            var viewModel = new RecentProjectsViewModel
            {
                Projects = projectsList,
                Count = count
            };

            return View(viewModel);
        }
    }
}