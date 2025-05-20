using System.Collections.Generic;
using System.Threading.Tasks;
using TaskDependency = SmartTask.Core.Models.TaskDependency;


namespace SmartTask.Core.IRepositories
{
    public interface ITaskDependencyRepository
    {
        Task<IEnumerable<TaskDependency>> GetAllAsync();
        Task<TaskDependency> GetByIdAsync(int id);
        Task<IEnumerable<TaskDependency>> GetByPredecessorIdAsync(int predecessorId);
        Task<IEnumerable<TaskDependency>> GetBySuccessorIdAsync(int successorId);
        Task<TaskDependency> AddAsync(TaskDependency taskDependency);
        Task DeleteAsync(int id);
        public Task DeleteRangeAsync(List<TaskDependency> td);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByTaskIdsAsync(int predecessorId, int successorId);
    }
}