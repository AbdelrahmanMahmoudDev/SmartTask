using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.Bl.Services;
using SmartTask.Core.IExternalServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.BL.Services;
using SmartTask.Web.CustomFilter;
using SmartTask.DataAccess.Data;
using SmartTask.DataAccess.ExternalServices;
using SmartTask.DataAccess.Repositories;
using SmartTask.Bl.Hubs;
using SmartTask.Bl.IServices;
using Microsoft.AspNetCore.Identity;
using System;
using task=System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Authorization;
using SmartTask.Web.Authorization.Handlers;
using SmartTask.Web.Authorization.Requirements;
using SmartTask.Web.Authorization;

namespace SmartTask.Web
{
    public class Program
    {
        public static async task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Core MVC services configuration
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(DynamicAuthorizationFilter));
            });


            //signal R
            builder.Services.AddSignalR();

            // Database & Identity
            builder.Services.AddDbContext<SmartTaskContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Authorizing Users
            builder.Services.AddAuthorization(options =>
            {
                AuthorizationPolicyConfiguration.ConfigureAuthorizationPolicies(options);
            });

            // HttpContextAccessor (required for the authorization handler)
            builder.Services.AddHttpContextAccessor();

            // Register authorization handlers
            AuthorizationPolicyConfiguration.RegisterAuthorizationHandlers(builder.Services);

            // Register the authorization handler
            builder.Services.AddScoped<IAuthorizationHandler, EditUserAuthorizationHandler>();

            // Database context configuration
            builder.Services.AddDbContext<SmartTaskContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
     .AddEntityFrameworkStores<SmartTaskContext>()
     .AddDefaultTokenProviders();


            // Dependency Injection
            RegisterRepositories(builder.Services);

            builder.Services.AddScoped(typeof(IPaginatedService<>), typeof(PaginatedService<>));

            // IUser service
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();

            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<SmartTaskContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }

            // Error Handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Handling UnAuthorized Access
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 403)
                {
                    context.HttpContext.Response.Redirect("/Error/AccessDenied");
                }
            });

            // Middleware Pipeline
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpoints
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<IMvcControllerDiscovery, MvcControllerDiscovery>();
            services.AddSingleton(new DynamicAuthorizationOptions { DefaultAdminUser = "aelashry@outlook.com" });
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();

            // Repository Interfaces to Implementations
         
            services.AddScoped<IAISuggestionRepository, AISuggestionRepository>();
            services.AddScoped<IAssignTaskRepository, AssignTaskRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IBranchDepartmentRepository, BranchDepartmentRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            //services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            
            //services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            //services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITaskDependencyRepository, TaskDependencyRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IProjectService, ProjectService>();

        }
    }
}