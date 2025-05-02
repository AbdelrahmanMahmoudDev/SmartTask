using System.Collections.Generic;
using System.Threading.Tasks;

using ModelTask = SmartTask.Core.Models.Task;


namespace SmartTask.Core.IRepositories
{
    public interface ITaskRepository
    {

        Task<IEnumerable<ModelTask>> GetAllAsync();
        Task<ModelTask> GetByIdAsync(int id);
        Task<ModelTask> GetWithDetailsAsync(int id);
        Task<IEnumerable<ModelTask>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<ModelTask>> GetByAssignedToIdAsync(int userId);
        Task<IEnumerable<ModelTask>> GetByParentTaskIdAsync(int parentTaskId);
        Task<IEnumerable<ModelTask>> GetByCreatedByIdAsync(int userId);
        Task<IEnumerable<ModelTask>> GetTasksByStatusAsync(string status);
        Task<IEnumerable<ModelTask>> GetTasksByPriorityAsync(string priority);
        Task<ModelTask> AddAsync(ModelTask task);
        Task UpdateAsync(ModelTask task);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}