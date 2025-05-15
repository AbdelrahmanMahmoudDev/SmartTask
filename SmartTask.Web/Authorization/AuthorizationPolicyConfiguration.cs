using Microsoft.AspNetCore.Authorization;
using SmartTask.Web.Authorization.Handlers;
using SmartTask.Web.Authorization.Requirements;

namespace SmartTask.Web.Authorization
{
    public static class AuthorizationPolicyConfiguration
    {
        public static void ConfigureAuthorizationPolicies(AuthorizationOptions options)
        {
            // User management policies
            options.AddPolicy("CanEditUser", policy =>
                policy.Requirements.Add(new EditUserRequirement()));

            // Project management policies
            options.AddPolicy("CanViewProject", policy =>
                policy.Requirements.Add(new ViewProjectRequirement()));

            options.AddPolicy("CanEditProject", policy =>
                policy.Requirements.Add(new EditProjectRequirement()));

            options.AddPolicy("CanDeleteProject", policy =>
                policy.Requirements.Add(new DeleteProjectRequirement()));

            options.AddPolicy("CanManageProjectMembers", policy =>
                policy.Requirements.Add(new ManageProjectMembersRequirement()));

            // Role-based policies
            options.AddPolicy("RequireAdminRole", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("RequireProjectManagerRole", policy =>
                policy.RequireRole("Admin", "ProjectManager"));
        }

        public static void RegisterAuthorizationHandlers(IServiceCollection services)
        {
            // Register handlers
            services.AddScoped<IAuthorizationHandler, EditUserAuthorizationHandler>();

            // Register separate handlers for each project requirement
            services.AddScoped<IAuthorizationHandler, ViewProjectAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, EditProjectAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, DeleteProjectAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, ManageProjectMembersAuthorizationHandler>();
        }
    }
}