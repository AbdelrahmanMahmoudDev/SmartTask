using System.Collections.Generic;
using System.Threading.Tasks;
using AssignTask = SmartTask.Core.Models.AssignTask;

namespace SmartTask.Core.IRepositories
{
    public interface IAssignTaskRepository
    {
        Task<IEnumerable<AssignTask>> GetAllAsync();
        Task<AssignTask> GetByIdAsync(int taskId, string userId);
        Task<IEnumerable<AssignTask>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<AssignTask>> GetByUserIdAsync(string userId);
        Task<AssignTask> AddAsync(AssignTask assignTask);
        Task UpdateAsync(AssignTask assignTask);
        Task DeleteAsync(int taskId, string userId);
        Task<bool> ExistsAsync(int taskId, string userId);
    }
}
