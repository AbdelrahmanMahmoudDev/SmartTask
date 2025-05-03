using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using ProjectMember = SmartTask.Core.Models.ProjectMember;


namespace SmartTask.DataAccess.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly SmartTaskContext _context;

        public ProjectMemberRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectMember>> GetAllAsync()
        {
            return await _context.ProjectMembers
                .Include(pm => pm.Project)
                .Include(pm => pm.User)
                .ToListAsync();
        }

        public async Task<ProjectMember> GetByIdsAsync(int projectId, string userId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.Project)
                .Include(pm => pm.User)
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }

        public async Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(int projectId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.User)
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectMember>> GetByUserIdAsync(string userId)
        {
            return await _context.ProjectMembers
                .Include(pm => pm.Project)
                .Where(pm => pm.UserId == userId)
                .ToListAsync();
        }

        //public async Task<IEnumerable<ProjectMember>> GetByProjectRoleIdAsync(int projectRoleId)
        //{
        //    return await _context.ProjectMembers
        //        .Include(pm => pm.Project)
        //        .Include(pm => pm.User)
        //        .Where(pm => pm.ProjectRoleId == projectRoleId)
        //        .ToListAsync();
        //}

        public async Task AddAsync(ProjectMember projectMember)
        {
            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectMember projectMember)
        {
            _context.Entry(projectMember).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int projectId, string userId)
        {
            var entity = await _context.ProjectMembers.FindAsync(projectId, userId);
            if (entity != null)
            {
                _context.ProjectMembers.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int projectId, string userId)
        {
            return await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        }
    }
}