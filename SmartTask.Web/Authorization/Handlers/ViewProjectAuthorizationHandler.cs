using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Web.Authorization.Requirements;

namespace SmartTask.Web.Authorization.Handlers
{
    public class ViewProjectAuthorizationHandler : AuthorizationHandler<ViewProjectRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectService _projectService;

        public ViewProjectAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            IProjectService projectService)
        {
            _httpContextAccessor = httpContextAccessor;
            _projectService = projectService;
        }

        protected override async System.Threading.Tasks.Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ViewProjectRequirement requirement)
        {
            // Get project ID from route
            if (!TryGetProjectIdFromRoute(out var projectId))
            {
                return;
            }

            // Admins can view any project
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Get current user
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Check if user is a member of the project
            var project = await _projectService.GetProjectByIdAsync(projectId);
            if (project != null &&
                (project.OwnerId == userId ||
                 project.ProjectMembers.Any(pm => pm.UserId == userId)))
            {
                context.Succeed(requirement);
            }
        }

        // Helper method to get project ID from route
        private bool TryGetProjectIdFromRoute(out int projectId)
        {
            projectId = 0;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return false;
            }

            var routeData = httpContext.Request.RouteValues;
            var idValue = routeData["id"]?.ToString();

            // Try to get id from route
            if (string.IsNullOrEmpty(idValue) || !int.TryParse(idValue, out projectId))
            {
                return false;
            }

            return true;
        }
    }
}