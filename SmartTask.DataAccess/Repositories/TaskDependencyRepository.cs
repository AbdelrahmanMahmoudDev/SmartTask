using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using TaskDependency = SmartTask.Core.Models.TaskDependency;


namespace SmartTask.DataAccess.Repositories
{
    public class TaskDependencyRepository : ITaskDependencyRepository
    {
        private readonly SmartTaskContext _context;

        public TaskDependencyRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskDependency>> GetAllAsync()
        {
            return await _context.TaskDependencies
                .Include(td => td.Predecessor)
                .Include(td => td.Successor)
                .ToListAsync();
        }

        public async Task<TaskDependency> GetByIdAsync(int id)
        {
            return await _context.TaskDependencies
                .Include(td => td.Predecessor)
                .Include(td => td.Successor)
                .FirstOrDefaultAsync(td => td.Id == id);
        }

        public async Task<IEnumerable<TaskDependency>> GetByPredecessorIdAsync(int predecessorId)
        {
            return await _context.TaskDependencies
                .Include(td => td.Successor)
                .Where(td => td.PredecessorId == predecessorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskDependency>> GetBySuccessorIdAsync(int successorId)
        {
            return await _context.TaskDependencies
                .Include(td => td.Predecessor)
                .Where(td => td.SuccessorId == successorId)
                .ToListAsync();
        }

        public async Task<TaskDependency> AddAsync(TaskDependency taskDependency)
        {
            _context.TaskDependencies.Add(taskDependency);
            await _context.SaveChangesAsync();
            return taskDependency;
        }

        public async Task DeleteAsync(int id)
        {
            var taskDependency = await _context.TaskDependencies.FindAsync(id);
            if (taskDependency != null)
            {
                _context.TaskDependencies.Remove(taskDependency);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteRangeAsync(List<TaskDependency> td)
        {
            _context.TaskDependencies.RemoveRange(td);
            await _context.SaveChangesAsync();
            
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TaskDependencies.AnyAsync(td => td.Id == id);
        }

        public async Task<bool> ExistsByTaskIdsAsync(int predecessorId, int successorId)
        {
            return await _context.TaskDependencies
                .AnyAsync(td => td.PredecessorId == predecessorId && td.SuccessorId == successorId);
        }
    }
}