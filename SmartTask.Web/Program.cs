using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.BL.Services.NotificationService;
using SmartTask.DataAccess.ExternalServices.EmailService;
using SmartTask.Core.IRepositories;
using SmartTask.DataAccess.Repositories;
using SmartTask.DataAccess.Data;
using SmartTask.Core.Models.Mail;
using SmartTask.Core.Models;
using SmartTask.Core.IExternalServices;

namespace SmartTask.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Core MVC services configuration
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));

            // Database context configuration
            builder.Services.AddDbContext<SmartTaskContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SmartTaskContext>();

            // Dependency injection registrations
            RegisterRepositories(builder.Services);

            var app = builder.Build();

            // Middleware pipeline configuration
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Endpoint routing configuration
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<INotificationService, NotificationService>();
            // Repository layer DI registrations
            services.AddScoped<  IAISuggestionRepository, AISuggestionRepository> ();
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