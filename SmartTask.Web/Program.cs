using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTask.Bl.IServices;
using SmartTask.Bl.Services;
using SmartTask.BL.IServices;
using SmartTask.BL.Service.Hubs;
using SmartTask.BL.Services;
using SmartTask.Core.IExternalServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.DataAccess.Data;
using SmartTask.DataAccess.ExternalServices;
using SmartTask.DataAccess.Repositories;
using SmartTask.Web.CustomFilter;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using task = System.Threading.Tasks.Task;

namespace SmartTask.Web
{
    public class Program
    {
        public static async task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region MVC & Filters

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(DynamicAuthorizationFilter));
            });

            #endregion MVC & Filters

            #region SignalR

            builder.Services.AddSignalR();

            #endregion SignalR

            #region Database & Identity

            builder.Services.AddDbContext<SmartTaskContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<SmartTaskContext>();

            #endregion Database & Identity

            #region Authentication

            builder.Services.AddAuthentication()
                .AddCookie()
                .AddMicrosoftAccount("Outlook", options =>
                {
                    options.ClientId = builder.Configuration["AzureAd:ClientId"];
                    options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
                    options.CallbackPath = builder.Configuration["AzureAd:CallbackPath"];
                    options.SaveTokens = true;
                    options.Scope.Add("offline_access");
                    options.Scope.Add("User.Read");
                    options.Scope.Add("Calendars.Read");
                });

            #endregion Authentication

            #region Dependency Injection and Others

            RegisterRepositories(builder.Services);

            builder.Services.AddScoped(typeof(IPaginatedService<>), typeof(PaginatedService<>));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();
            #endregion
            // IUser service
            builder.Services.AddScoped<IUserService, UserService>();
            // extendProjectDeadline Based ON Task Service
            builder.Services.AddScoped<IProjectDeadlineExtendService, ProjectDeadlineExtendService>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            var app = builder.Build();

            #region Session and Migration

            app.UseSession();

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

            #endregion Session and Migration

            #region Middlewares

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            #endregion Middlewares

            #region Endpoints

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapHub<NotificationHub>("/notificationHub");

            #endregion Endpoints

            app.Run();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<IMvcControllerDiscovery, MvcControllerDiscovery>();
            services.AddSingleton(new DynamicAuthorizationOptions
            {
                DefaultAdminUser = "ahmedramadan.l403@gmail.com"
            });

            // Services
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITaskService, TaskService>();
            //services.AddScoped<ITaskService, TaskService>();

            // Repositories
            services.AddScoped<IAISuggestionRepository, AISuggestionRepository>();
            services.AddScoped<IAssignTaskRepository, AssignTaskRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IBranchDepartmentRepository, BranchDepartmentRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskDependencyRepository, TaskDependencyRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IUserColumnPreferenceRepository, UserColumnPreferenceRepository>();
            services.AddScoped<IUserColumnPreferenceService, UserColumnPreferenceService>();
           


            services.AddScoped<INotificationRepository, NotificationRepository>();
        }
    }
}