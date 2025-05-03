using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using AssignTask = SmartTask.Core.Models.AssignTask;






namespace SmartTask.DataAccess.Repositories
{ 
    public class AssignTaskRepository : IAssignTaskRepository
    {
        private readonly SmartTaskContext _context;

        public AssignTaskRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssignTask>> GetAllAsync()
        {
            return await _context.AssignTasks
                .Include(a => a.Task)
                .Include(a => a.User)
                .Include(a => a.AssignedBy)
                .ToListAsync();
        }

        public async Task<AssignTask> GetByIdAsync(int taskId, string userId)
        {
            return await _context.AssignTasks
                .Include(a => a.Task)
                .Include(a => a.User)
                .Include(a => a.AssignedBy)
                .FirstOrDefaultAsync(a => a.TaskId == taskId && a.UserId == userId);
        }

        public async Task<IEnumerable<AssignTask>> GetByTaskIdAsync(int taskId)
        {
            return await _context.AssignTasks
                .Include(a => a.User)
                .Include(a => a.AssignedBy)
                .Where(a => a.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignTask>> GetByUserIdAsync(string userId)
        {
            return await _context.AssignTasks
                .Include(a => a.Task)
                .Include(a => a.AssignedBy)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<AssignTask> AddAsync(AssignTask assignTask)
        {
            _context.AssignTasks.Add(assignTask);
            await _context.SaveChangesAsync();
            return assignTask;
        }

        public async Task UpdateAsync(AssignTask assignTask)
        {
            _context.Entry(assignTask).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int taskId, string userId)
        {
            var assignTask = await _context.AssignTasks.FindAsync(taskId, userId);
            if (assignTask != null)
            {
                _context.AssignTasks.Remove(assignTask);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int taskId, string userId)
        {
            return await _context.AssignTasks
                .AnyAsync(a => a.TaskId == taskId && a.UserId == userId);
        }

    }
}