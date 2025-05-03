using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.Bl.Services;
using SmartTask.Core.IExternalServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.Mail;
using SmartTask.DataAccess.Data;
using SmartTask.DataAccess.ExternalServices;
using SmartTask.DataAccess.Repositories;
using SmartTask.Bl.Hubs;

namespace SmartTask.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC & Configuration
            builder.Services.AddControllersWithViews();
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


            //signal R
            builder.Services.AddSignalR();

            // Database & Identity
            builder.Services.AddDbContext<SmartTaskContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<SmartTaskContext>().AddDefaultTokenProviders();

            // Dependency Injection
            RegisterRepositories(builder.Services);

            var app = builder.Build();

            // Error Handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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
            // External Services
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
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectRolePermissionRepository, ProjectRolePermissionRepository>();
            services.AddScoped<IProjectRoleRepository, ProjectRoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITaskDependencyRepository, TaskDependencyRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}