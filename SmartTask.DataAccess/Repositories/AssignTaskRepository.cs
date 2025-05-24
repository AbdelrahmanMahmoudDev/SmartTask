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
        public async Task<List<AssignTask>> FindTasksAssignedToUserByIds(List<string> ids)
        {
            return _context.AssignTasks
                .Where(a => ids.Contains(a.UserId))
                .ToList();
        }
        //public async Task AssignTasksToUserByIds(List<string> ids, TaskModel task, ClaimsPrincipal user)
        //{
        //    //var assignedTasks = new List<AssignTask>();
        //    AssignTask assignedTask;

        //    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        //    foreach (var id in ids)
        //    {
        //        assignedTask = new AssignTask
        //        {
        //            UserId = id,
        //            TaskId = task.Id,
        //            AssignedAt = DateTime.UtcNow,
        //            AssignedById = userId,
        //            Status = task.Status,
        //            Comments = ""
        //        };
        //        _context.AssignTasks.Add(assignedTask);

        //    }
        //    //_context.AssignTasks.AddRange(assignedTasks);
        //    await _context.SaveChangesAsync();
        //}
        //public async Task ModifyTasksToUserByIds(string userId, TaskModel _task, List<string> assignments)
        //{

        //    _task.CreatedById = userId;
        //    _task.CreatedAt = DateTime.Now;
        //    _task.UpdatedAt = DateTime.Now;
        //    _context.AssignTasks.RemoveRange(_task.Assignments);
        //    await _context.SaveChangesAsync();

        //    foreach (var assignedUserId in assignments)
        //    {
        //        _context.AssignTasks.Add(new AssignTask
        //        {
        //            UserId = assignedUserId,
        //            TaskId = _task.Id,
        //            AssignedAt = DateTime.UtcNow,
        //            AssignedById = userId,
        //            Status = _task.Status,
        //            Comments = ""
        //        });
        //    }
        //    await _context.SaveChangesAsync();
        //}
    }
}