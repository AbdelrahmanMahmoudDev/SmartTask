using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using Project = SmartTask.Core.Models.Project;
using SmartTask.Core.Models;


namespace SmartTask.DataAccess.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly SmartTaskContext _context;

        public ProjectRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.CreatedBy)
                .ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetAllAsyncWithoutInclude()
        {
            return await _context.Projects
                .ToListAsync();
        }

        public async Task<Project> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project> GetWithDetailsAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User) 
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public  List<ApplicationUser> GetMembers(int id)
        {
            return  _context.Projects
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                    .Where(p => p.Id == id)
                    .Select(p => p.ProjectMembers.Select(pm => pm.User).ToList())
                    .FirstOrDefault();
        }
        public async Task<IEnumerable<Project>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.Projects
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByCreatedByIdAsync(string createdById)
        {
            return await _context.Projects
                .Where(p => p.CreatedById == createdById)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(string userId)
        {
            return await _context.ProjectMembers
                .Where(pm => pm.UserId == userId)
                .Select(pm => pm.Project)
                .Include(p => p.Owner)
                .ToListAsync();
        }

        public async Task<Project> AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async System.Threading.Tasks.Task UpdateAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Projects.AnyAsync(p => p.Id == id);
        }

        public IQueryable<Project> GetQueryable()
        {
            return _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.CreatedBy)
                .AsQueryable();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            return await _context.Projects
                .Include(p => p.ProjectMembers)
                .Include(p => p.Tasks)
                .Where(p => p.ProjectMembers.Any(pm => pm.UserId == userId))
                .ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int id, string userId)
        {
            return await _context.Projects
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id &&
                    (p.ProjectMembers.Any(pm => pm.UserId == userId)));
        }
    }
}