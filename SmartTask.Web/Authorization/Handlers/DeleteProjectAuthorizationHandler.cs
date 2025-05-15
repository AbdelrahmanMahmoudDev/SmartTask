using Microsoft.AspNetCore.Authorization;
using SmartTask.BL.IServices;
using SmartTask.Web.Authorization.Requirements;
using System.Security.Claims;

namespace SmartTask.Web.Authorization.Handlers
{
    public class DeleteProjectAuthorizationHandler : AuthorizationHandler<DeleteProjectRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectService _projectService;

        public DeleteProjectAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            IProjectService projectService)
        {
            _httpContextAccessor = httpContextAccessor;
            _projectService = projectService;
        }

        protected override async System.Threading.Tasks.Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DeleteProjectRequirement requirement)
        {
            // Get project ID from route
            if (!TryGetProjectIdFromRoute(out var projectId))
            {
                return;
            }

            // Admins can delete any project
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

            // Project Managers can delete projects they own or are members of
            if (context.User.IsInRole("ProjectManager"))
            {
                var project = await _projectService.GetProjectByIdAsync(projectId);
                if (project != null &&
                    (project.OwnerId == userId ||
                     project.ProjectMembers.Any(pm => pm.UserId == userId)))
                {
                    context.Succeed(requirement);
                }
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
