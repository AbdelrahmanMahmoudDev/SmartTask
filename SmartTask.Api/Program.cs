using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartTask.Bl.IServices;
using SmartTask.Bl.Services;
using SmartTask.BL.IServices;
using SmartTask.BL.Services;
using SmartTask.Core.IRepositories;
using SmartTask.Core.IExternalServices;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Core.Models.Mail;
using SmartTask.DataAccess.Data;
using SmartTask.DataAccess.ExternalServices;
using SmartTask.DataAccess.Repositories;
using task = System.Threading.Tasks.Task;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SmartTask.Api
{
    public class Program
    {
        public static async task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Basic Services
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            #endregion

            #region Database & Identity
            builder.Services.AddDbContext<SmartTaskContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<SmartTaskContext>();
            #endregion

            #region JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:5086/",
                    ValidateAudience = true,
                    ValidAudience = "http://localhost:4200/",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("sdfljsdlfj9o4oieurwew//cv??fdssdrer///???430958dlsjfkjdssdfl||dsf"))
                };
            });
            #endregion

            #region Swagger
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 5 Web API",
                    Description = "Security"
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token."
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            #endregion

            #region Dependency Injection

            // Core Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped(typeof(IPaginatedService<>), typeof(PaginatedService<>));

            // Email Sender
            builder.Services.AddScoped<IEmailSender, EmailService>();

            // Repositories
            builder.Services.AddScoped<IAISuggestionRepository, AISuggestionRepository>();
            builder.Services.AddScoped<IAssignTaskRepository, AssignTaskRepository>();
            builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            builder.Services.AddScoped<IBranchDepartmentRepository, BranchDepartmentRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<ITaskDependencyRepository, TaskDependencyRepository>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
            builder.Services.AddScoped<IAuditRepository, AuditRepository>();

            // Dynamic Authorization
            builder.Services.AddSingleton<IMvcControllerDiscovery, MvcControllerDiscovery>();
            builder.Services.AddSingleton(new DynamicAuthorizationOptions
            {
                DefaultAdminUser = "aelashry@outlook.com"
            });

            #endregion

            builder.Services.AddControllers()
                                            .AddJsonOptions(options =>
                                            {
                                                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                                                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                                                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                                                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                            });

            var app = builder.Build();

            // Apply Migrations
            using (var scope = app.Services.CreateScope())
            {
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
            }

            #region Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
