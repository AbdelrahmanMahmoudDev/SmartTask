using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using ProjectRole = SmartTask.Core.Models.ProjectRole;


namespace SmartTask.DataAccess.Repositories
{
    public class ProjectRoleRepository : IProjectRoleRepository
    {
        private readonly SmartTaskContext _context;

        public ProjectRoleRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectRole>> GetAllAsync()
        {
            return await _context.ProjectRoles
                .Include(pr => pr.Project)
                .ToListAsync();
        }

        public async Task<ProjectRole> GetByIdAsync(int id)
        {
            return await _context.ProjectRoles
                .Include(pr => pr.Project)
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task<ProjectRole> GetWithDetailsAsync(int id)
        {
            return await _context.ProjectRoles
                .Include(pr => pr.Project)
                .Include(pr => pr.ProjectRolePermissions)
                    .ThenInclude(prp => prp.Permission)
                .Include(pr => pr.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .FirstOrDefaultAsync(pr => pr.Id == id);
        }

        public async Task<IEnumerable<ProjectRole>> GetByProjectIdAsync(int projectId)
        {
            return await _context.ProjectRoles
                .Include(pr => pr.ProjectRolePermissions)
                .Where(pr => pr.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<ProjectRole> AddAsync(ProjectRole projectRole)
        {
            _context.ProjectRoles.Add(projectRole);
            await _context.SaveChangesAsync();
            return projectRole;
        }

        public async Task UpdateAsync(ProjectRole projectRole)
        {
            _context.Entry(projectRole).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ProjectRoles.FindAsync(id);
            if (entity != null)
            {
                _context.ProjectRoles.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ProjectRoles.AnyAsync(pr => pr.Id == id);
        }
    }
}