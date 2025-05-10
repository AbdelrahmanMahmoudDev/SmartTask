using System.Collections.Generic;
using System.Threading.Tasks;
using Project = SmartTask.Core.Models.Project;

namespace SmartTask.Core.IRepositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Project>> GetAllAsyncWithoutInclude();
        Task<Project> GetByIdAsync(int id);
        Task<Project> GetWithDetailsAsync(int id);
        Task<IEnumerable<Project>> GetByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Project>> GetByCreatedByIdAsync(string createdById);
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(string userId);
        Task<Project> AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}