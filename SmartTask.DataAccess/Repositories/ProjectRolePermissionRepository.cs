using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using ProjectRolePermission = SmartTask.Core.Models.ProjectRolePermission;


namespace SmartTask.DataAccess.Repositories
{
    public class ProjectRolePermissionRepository : IProjectRolePermissionRepository
    {
        private readonly SmartTaskContext _context;

        public ProjectRolePermissionRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectRolePermission>> GetAllAsync()
        {
            return await _context.ProjectRolePermissions
                .Include(prp => prp.ProjectRole)
                .Include(prp => prp.Permission)
                .ToListAsync();
        }

        public async Task<ProjectRolePermission> GetByIdsAsync(int projectRoleId, int permissionId)
        {
            return await _context.ProjectRolePermissions
                .Include(prp => prp.ProjectRole)
                .Include(prp => prp.Permission)
                .FirstOrDefaultAsync(prp => prp.ProjectRoleId == projectRoleId && prp.PermissionId == permissionId);
        }

        public async Task<IEnumerable<ProjectRolePermission>> GetByProjectRoleIdAsync(int projectRoleId)
        {
            return await _context.ProjectRolePermissions
                .Include(prp => prp.Permission)
                .Where(prp => prp.ProjectRoleId == projectRoleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectRolePermission>> GetByPermissionIdAsync(int permissionId)
        {
            return await _context.ProjectRolePermissions
                .Include(prp => prp.ProjectRole)
                .Where(prp => prp.PermissionId == permissionId)
                .ToListAsync();
        }

        public async Task AddAsync(ProjectRolePermission projectRolePermission)
        {
            _context.ProjectRolePermissions.Add(projectRolePermission);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int projectRoleId, int permissionId)
        {
            var entity = await _context.ProjectRolePermissions
                .FirstOrDefaultAsync(prp => prp.ProjectRoleId == projectRoleId && prp.PermissionId == permissionId);
            
            if (entity != null)
            {
                _context.ProjectRolePermissions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int projectRoleId, int permissionId)
        {
            return await _context.ProjectRolePermissions
                .AnyAsync(prp => prp.ProjectRoleId == projectRoleId && prp.PermissionId == permissionId);
        }
    }
}