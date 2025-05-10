using System.Collections.Generic;
using System.Threading.Tasks;
using SmartTask.Core.Models.Enums;
using ModelTask = SmartTask.Core.Models.Task;


namespace SmartTask.Core.IRepositories
{
    public interface ITaskRepository
    {

        Task<IEnumerable<ModelTask>> GetAllAsync();
        Task<ModelTask> GetByIdAsync(int id);
        Task<ModelTask> GetWithDetailsAsync(int id);
        Task<IEnumerable<ModelTask>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<ModelTask>> GetByAssignedToIdAsync(string userId);
        Task<IEnumerable<ModelTask>> GetByParentTaskIdAsync(int parentTaskId);
        Task<IEnumerable<ModelTask>> GetByCreatedByIdAsync(string userId);
        Task<IEnumerable<ModelTask>> GetTasksByStatusAsync(Status status);
        Task<IEnumerable<ModelTask>> GetTasksByPriorityAsync(Priority priority);
        Task AddAsync(ModelTask task);
        Task UpdateAsync(ModelTask task);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<ModelTask>> GetAllTasksPerProject(int projectId);
    }
}