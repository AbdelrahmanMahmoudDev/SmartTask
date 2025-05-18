using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SmartTask.Web.Authorization.Requirements;

namespace SmartTask.Web.Authorization.Handlers
{
    public class EditUserAuthorizationHandler : AuthorizationHandler<EditUserRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EditUserRequirement requirement)
        {
            // Get the "id" from route values
            var routeData = _httpContextAccessor.HttpContext?.Request.RouteValues;
            var userId = routeData?["id"]?.ToString();

            if (string.IsNullOrEmpty(userId))
            {
                // If no id in route, we can't verify - deny access
                return Task.CompletedTask;
            }

            // Get current user's ID
            var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if user is admin
            var isAdmin = context.User.IsInRole("Admin");

            // Allow if user is admin or editing their own profile
            if (isAdmin || userId == currentUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
