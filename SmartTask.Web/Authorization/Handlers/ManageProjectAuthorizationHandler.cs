using Microsoft.AspNetCore.Authorization;
using SmartTask.BL.IServices;
using SmartTask.Web.Authorization.Requirements;
using System.Security.Claims;

namespace SmartTask.Web.Authorization.Handlers
{
    public class ManageProjectMembersAuthorizationHandler : AuthorizationHandler<ManageProjectMembersRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectService _projectService;

        public ManageProjectMembersAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            IProjectService projectService)
        {
            _httpContextAccessor = httpContextAccessor;
            _projectService = projectService;
        }

        protected override async System.Threading.Tasks.Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ManageProjectMembersRequirement requirement)
        {
            // Get project ID from route or form
            var (success, projectId) = await TryGetProjectIdFromRouteOrFormAsync();
            if (!success)
            {
                return;
            }

            // Admins can manage members for any project
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

            // Project Managers can manage members for projects they own or are members of
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

        // Helper method to get project ID from route or form data
        private async System.Threading.Tasks.Task<(bool success, int projectId)> TryGetProjectIdFromRouteOrFormAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return (false, 0);
            }

            // Try to get projectId from form data first (for POST requests)
            if (httpContext.Request.Method == "POST" &&
                httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();
                var projectIdStr = form["projectId"].ToString();
                if (!string.IsNullOrEmpty(projectIdStr) && int.TryParse(projectIdStr, out int projectId))
                {
                    return (true, projectId);
                }
            }

            // Fall back to route data
            var routeResult = TryGetProjectIdFromRoute(out int routeProjectId);
            return (routeResult, routeProjectId);
        }
    }
}
