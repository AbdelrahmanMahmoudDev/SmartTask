using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SmartTask.Core.Models.BasePermission;
using SmartTask.DataAccess.Data;
using Newtonsoft.Json;
using System.Reflection;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;

namespace SmartTask.Web.CustomFilter
{
    public class DynamicAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly SmartTaskContext _dbContext;
        private readonly DynamicAuthorizationOptions _authorizationOptions;

        public DynamicAuthorizationFilter(SmartTaskContext dbContext, DynamicAuthorizationOptions authorizationOptions)
        {
            _dbContext = dbContext;
            _authorizationOptions = authorizationOptions;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!IsProtectedAction(context))
                return;

            if (!IsUserAuthenticated(context))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userName = context.HttpContext.User.Identity.Name;
            if (userName.Equals(_authorizationOptions.DefaultAdminUser, StringComparison.CurrentCultureIgnoreCase))
                return;

            var actionId = GetActionId(context);

            var roles = await (
                from user in _dbContext.Users
                join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                where user.UserName == userName
                select role
            ).ToListAsync();

            foreach (var role in roles)
            {
                if (role.Access == null)
                    continue;

                var accessList = JsonConvert.DeserializeObject<IEnumerable<MvcControllerInfo>>(role.Access);
                if (accessList.SelectMany(c => c.Actions).Any(a => a.Id == actionId))
                    return;
            }

            context.Result = new ForbidResult();
        }

        private static bool IsProtectedAction(AuthorizationFilterContext context)
        {
            var actiontype = context.ActionDescriptor.GetType();
            // var controllerActionDescriptor = context.ActionDescriptor;
            if (actiontype.FullName == "Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor")
            {
                var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
                var controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
                var anonymousAttribute = controllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>();
                if (anonymousAttribute != null)
                    return false;
                var actionMethodInfo = controllerActionDescriptor.MethodInfo;
                anonymousAttribute = actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>();
                if (anonymousAttribute != null)
                    return false;

                var authorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>();
                if (authorizeAttribute != null)
                    return true;

                authorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>();
                if (authorizeAttribute != null)
                    return true;

                return false;
            }
            else
            {
                //razorpage
                var controllerActionDescriptor = (CompiledPageActionDescriptor)context.ActionDescriptor;
                var controllerTypeInfo = controllerActionDescriptor.HandlerTypeInfo;
                var anonymousAttribute = controllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>();
                if (anonymousAttribute != null)
                    return false;
                var actionMethodInfo = controllerActionDescriptor.HandlerTypeInfo;
                anonymousAttribute = actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>();
                if (anonymousAttribute != null)
                    return false;

                var authorizeAttribute = controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>();
                if (authorizeAttribute != null)
                    return true;

                authorizeAttribute = actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>();
                if (authorizeAttribute != null)
                    return true;

                return false;
            }


        }

        private static bool IsUserAuthenticated(AuthorizationFilterContext context)
        {
            return context.HttpContext.User.Identity.IsAuthenticated;
        }

        private static string GetActionId(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var area = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue;
            var controller = controllerActionDescriptor.ControllerName;
            var action = controllerActionDescriptor.ActionName;

            return $"{area}:{controller}:{action}";
        }
    }
}
