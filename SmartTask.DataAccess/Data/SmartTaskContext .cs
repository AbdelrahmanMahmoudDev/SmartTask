using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Core.Models.AuditModels;
using TaskModel = SmartTask.Core.Models.Task;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace SmartTask.DataAccess.Data
{
    /// <summary>
    /// Represents the EF Core database context for SmartTask.
    /// Manages DbSet properties and configures entity relationships.
    /// </summary>
    public class SmartTaskContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SmartTaskContext(DbContextOptions<SmartTaskContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<TaskModel> Tasks { get; set; }
        //public DbSet<Role> RolesSmart { get; set; }
        //public DbSet<Permission> Permissions { get; set; }
        //public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<BranchDepartment> BranchDepartments { get; set; }
        public DbSet<Project> Projects { get; set; }
        //public DbSet<ProjectRole> ProjectRoles { get; set; }
        //public DbSet<ProjectRolePermission> ProjectRolePermissions { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TaskDependency> TaskDependencies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AISuggestion> AISuggestions { get; set; }
        public DbSet<AssignTask> AssignTasks { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<UserLoginHistory>UserLoginHistories { get; set; }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Primary Keys
            //modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<BranchDepartment>().HasKey(bd => new { bd.BranchId, bd.DepartmentId });
            //modelBuilder.Entity<ProjectRolePermission>().HasKey(prp => new { prp.ProjectRoleId, prp.PermissionId });
            modelBuilder.Entity<ProjectMember>().HasKey(pm => new { pm.ProjectId, pm.UserId });
            modelBuilder.Entity<AssignTask>().HasKey(at => new { at.TaskId, at.UserId });

            // Task Relationships
            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskDependency>()
                .HasOne(td => td.Predecessor)
                .WithMany(t => t.PredecessorDependencies)
                .HasForeignKey(td => td.PredecessorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskDependency>()
                .HasOne(td => td.Successor)
                .WithMany(t => t.SuccessorDependencies)
                .HasForeignKey(td => td.SuccessorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Branch & Department Managers
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Manager)
                .WithMany(u => u.ManagedBranches)
                .HasForeignKey(b => b.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithMany(u => u.ManagedDepartments)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project Ownership
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedBy)
                .WithMany(u => u.CreatedProjects)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Task Audit Fields
            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.UpdatedBy)
                .WithMany(u => u.UpdatedTasks)
                .HasForeignKey(t => t.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment History
            modelBuilder.Entity<AssignTask>()
                .HasOne(at => at.User)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(at => at.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignTask>()
                .HasOne(at => at.AssignedBy)
                .WithMany(u => u.TasksAssigned)
                .HasForeignKey(at => at.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Map User entity to a custom table if needed
            //modelBuilder.Entity<User>()
            //    .ToTable("Users");
        }

        //Auditing 
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var UserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            BeforeSaveChanges(UserId, UserName);
            return await base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges()
        {
            var UserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            BeforeSaveChanges(UserId, UserName);
            return base.SaveChanges();
        }
        private void BeforeSaveChanges(string UserId, string UserName)
        {
            ChangeTracker.DetectChanges();
            var AuditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }
                var AuditEntry = new AuditEntry();
                AuditEntry.UserId = UserId;
                AuditEntry.TableName = entry.Entity.GetType().Name;
                AuditEntry.Username = UserName;

                AuditEntries.Add(AuditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            AuditEntry.Action = AuditAction.Create;
                            AuditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            AuditEntry.Action = AuditAction.Delete;
                            AuditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                AuditEntry.Action = AuditAction.Update;
                                AuditEntry.OldValues[propertyName] = property.OriginalValue;
                                AuditEntry.NewValues[propertyName] = property.CurrentValue;
                                AuditEntry.AffectedColumns.Add(propertyName);
                            }
                            break;
                    }

                }

            }
            foreach (var AuditEntry in AuditEntries)
            {
                var audit = AuditEntry.ToAudit();
                Audits.Add(audit);
            }
        }
    }
}