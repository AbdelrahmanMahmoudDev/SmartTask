using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models.Enums;
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
            var task= await _context.Tasks.Where(t => t.Id == id)
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Include(t => t.ParentTask)
                .Include(t => t.CreatedBy)
                .Include(t => t.UpdatedBy)
                .Include(t => t.SubTasks)
                .Include(t => t.PredecessorDependencies)
                .Include(t => t.SuccessorDependencies)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .Include(t => t.Attachments)
                    .ThenInclude(a => a.UploadedBy)
                .Include(t => t.Events)
                    .ThenInclude(e => e.ImportedBy)
                .Include(t => t.AISuggestions)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync();
            foreach (var dep in task.PredecessorDependencies)
            {
                _context.Entry(dep).Reference(d => d.Successor).Load();
            }

            foreach (var dep in task.SuccessorDependencies)
            {
                _context.Entry(dep).Reference(d => d.Predecessor).Load();
            }
            return task;
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
                .Include(t => t.Assignments)
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

        public async Task<IEnumerable<ModelTask>> GetTasksByStatusAsync(Status status)
        {
            return await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignments)
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<ModelTask>> GetTasksByPriorityAsync(Priority priority)
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
        public async Task<bool> ISAParent(int id)
        {
            return await _context.Tasks.AnyAsync(t => t.ParentTaskId== id);
        }

        public async Task<IEnumerable<ModelTask>> GetAllTasksPerProject(int projectId)
        {

            return await _context.Tasks.Where(t => t.ProjectId == projectId).Include(t => t.Project).ToListAsync();
        }
    }
}