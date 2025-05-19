using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AssignTask = SmartTask.Core.Models.AssignTask;
using TaskModel = SmartTask.Core.Models.Task;

namespace SmartTask.Core.IRepositories
{
    public interface IAssignTaskRepository
    {
        Task<IEnumerable<AssignTask>> GetAllAsync();
        Task<AssignTask> GetByIdAsync(int taskId, string userId);
        Task<IEnumerable<AssignTask>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<AssignTask>> GetByUserIdAsync(string userId);
        List<string> GetUsersIdByTaskId(int taskId);
        Task<AssignTask> AddAsync(AssignTask assignTask);
        Task UpdateAsync(AssignTask assignTask);
        Task DeleteAsync(int taskId, string userId);
        Task<bool> ExistsAsync(int taskId, string userId);
        Task<List<AssignTask>> FindTasksAssignedToUserByIds(List<string> ids);
        Task AssignTasksToUserByIds(List<string> ids, TaskModel task, ClaimsPrincipal user);
        Task ModifyTasksToUserByIds(string userId, TaskModel _task, List<string> assignments);
    }
}
