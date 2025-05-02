using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using TaskModel = SmartTask.Core.Models.Task;

namespace SmartTask.DataAccess.Data
{
    /// <summary>
    /// Represents the EF Core database context for SmartTask.
    /// Manages DbSet properties and configures entity relationships.
    /// </summary>
    public class SmartTaskContext : DbContext
    {
        /// <summary>
        /// Initializes the context with the specified options.
        /// </summary>
        /// <param name="options">Options for configuring the context.</param>
        public SmartTaskContext(DbContextOptions<SmartTaskContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<BranchDepartment> BranchDepartments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectRole> ProjectRoles { get; set; }
        public DbSet<ProjectRolePermission> ProjectRolePermissions { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TaskDependency> TaskDependencies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AISuggestion> AISuggestions { get; set; }
        public DbSet<AssignTask> AssignTasks { get; set; }

        /// <summary>
        /// Configures entity mappings and relationships using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">Builder for configuring the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite PKs
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<BranchDepartment>()
                .HasKey(bd => new { bd.BranchId, bd.DepartmentId });
            modelBuilder.Entity<ProjectRolePermission>()
                .HasKey(prp => new { prp.ProjectRoleId, prp.PermissionId });
            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });
            modelBuilder.Entity<AssignTask>()
                .HasKey(at => new { at.TaskId, at.UserId });

            // Task self-reference
            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task dependencies
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

            // Branch-Department and User relations
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

            // Project ownership
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

            // Task audit fields
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

            // Assignment history
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
            modelBuilder.Entity<User>()
                .ToTable("Users");
        }
    }
}
