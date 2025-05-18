using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Core.Models.AuditModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskModel = SmartTask.Core.Models.Task;

namespace SmartTask.DataAccess.Data
{
    public class SmartTaskContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public SmartTaskContext(DbContextOptions<SmartTaskContext> options) : base(options)
        {
        }

        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<BranchDepartment> BranchDepartments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TaskDependency> TaskDependencies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AISuggestion> AISuggestions { get; set; }
        public DbSet<AssignTask> AssignTasks { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }

        public DbSet<UserDashboardPreference> UserDashboardPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Primary Keys
            modelBuilder.Entity<BranchDepartment>()
                .HasKey(bd => new { bd.BranchId, bd.DepartmentId });

            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });

            modelBuilder.Entity<AssignTask>()
                .HasKey(at => new { at.TaskId, at.UserId });

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

            // User-Branch-Department relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project-Branch-Department relationships
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Projects)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Branch)
                .WithMany(b => b.Projects)
                .HasForeignKey(p => p.BranchId)
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
        }

        public override int SaveChanges()
        {
            ValidateUserDepartmentBranchRelationship();
            ValidateProjectDepartmentBranchRelationship();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ValidateUserDepartmentBranchRelationship();
            ValidateProjectDepartmentBranchRelationship();
            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(string userId, string userName)
        {
            BeforeSaveChanges(userId, userName);
            ValidateUserDepartmentBranchRelationship();
            ValidateProjectDepartmentBranchRelationship();
            return await base.SaveChangesAsync();
        }

        private void ValidateUserDepartmentBranchRelationship()
        {
            var users = ChangeTracker.Entries<ApplicationUser>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            foreach (var user in users)
            {
                if (user.BranchId != null && user.DepartmentId != null)
                {
                    bool relationExists = BranchDepartments.Any(bd =>
                        bd.BranchId == user.BranchId &&
                        bd.DepartmentId == user.DepartmentId);

                    if (!relationExists)
                    {
                        throw new InvalidOperationException(
                            $"Department with Id {user.DepartmentId} is not associated with Branch Id {user.BranchId}. " +
                            "The department must be linked to the branch before assigning the user.");
                    }
                }
            }
        }

        private void ValidateProjectDepartmentBranchRelationship()
        {
            var projects = ChangeTracker.Entries<Project>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            foreach (var project in projects)
            {
                if (project.BranchId != null && project.DepartmentId != null)
                {
                    bool relationExists = BranchDepartments.Any(bd =>
                        bd.BranchId == project.BranchId &&
                        bd.DepartmentId == project.DepartmentId);

                    if (!relationExists)
                    {
                        throw new InvalidOperationException(
                            $"Department with Id {project.DepartmentId} is not associated with Branch Id {project.BranchId}. " +
                            "The department must be linked to the branch before assigning the project.");
                    }
                }
            }
        }

        private void BeforeSaveChanges(string userId, string userName)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var auditEntry = new AuditEntry
                {
                    UserId = userId,
                    TableName = entry.Entity.GetType().Name,
                    Username = userName
                };

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.Action = AuditAction.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.Action = AuditAction.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.Action = AuditAction.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.AffectedColumns.Add(propertyName);
                            }
                            break;
                    }
                }

                auditEntries.Add(auditEntry);
            }

            foreach (var auditEntry in auditEntries)
            {
                Audits.Add(auditEntry.ToAudit());
            }
        }
    }
}
