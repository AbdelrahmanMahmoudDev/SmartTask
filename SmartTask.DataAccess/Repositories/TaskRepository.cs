using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.DataAccess.Data;
using ModelTask = SmartTask.Core.Models.Task;



namespace SmartTask.DataAccess.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly SmartTaskContext _context;

        public TaskRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModelTask>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Include(t => t.CreatedBy)
                .ToListAsync();
        }

        public async Task<ModelTask> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Include(t => t.ParentTask)
                .Include(t => t.CreatedBy)
                .Include(t => t.UpdatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ModelTask> GetWithDetailsAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Include(t => t.ParentTask)
                .Include(t => t.CreatedBy)
                .Include(t => t.UpdatedBy)
                .Include(t => t.SubTasks)
                .Include(t => t.PredecessorDependencies)
                    .ThenInclude(td => td.Predecessor)
                .Include(t => t.SuccessorDependencies)
                    .ThenInclude(td => td.Successor)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .Include(t => t.Attachments)
                    .ThenInclude(a => a.UploadedBy)
                .Include(t => t.Events)
                    .ThenInclude(e => e.ImportedBy)
                .Include(t => t.AISuggestions)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<ModelTask>> GetByProjectIdAsync(int projectId)
        {
            return await _context.Tasks
                .Include(t => t.Assignments)
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModelTask>> GetByAssignedToIdAsync(string userId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
               .Where(t => t.Assignments.Any(a => a.UserId == userId))
        .ToListAsync();

        }

        public async Task<IEnumerable<ModelTask>> GetByParentTaskIdAsync(int parentTaskId)
        {
            return await _context.Tasks
                .Include(t => t.Assignments)
                .Where(t => t.ParentTaskId == parentTaskId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModelTask>> GetByCreatedByIdAsync(string userId)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Where(t => t.CreatedById == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModelTask>> GetTasksByStatusAsync(string status)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModelTask>> GetTasksByPriorityAsync(string priority)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Where(t => t.Priority == priority)
                .ToListAsync();
        }

        public async Task AddAsync(ModelTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateAsync(ModelTask task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tasks.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<ModelTask>> GetAllTasksPerProject(int projectId)
        {

            return await _context.Tasks.Where(t => t.ProjectId == projectId).Include(t => t.Project).ToListAsync();
        }
    }
}