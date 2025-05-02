using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectRole = SmartTask.Core.Models.ProjectRole;

namespace SmartTask.Core.IRepositories
{
    public interface IProjectRoleRepository
    {
        Task<IEnumerable<ProjectRole>> GetAllAsync();
        Task<ProjectRole> GetByIdAsync(int id);
        Task<ProjectRole> GetWithDetailsAsync(int id);
        Task<IEnumerable<ProjectRole>> GetByProjectIdAsync(int projectId);
        Task<ProjectRole> AddAsync(ProjectRole projectRole);
        Task UpdateAsync(ProjectRole projectRole);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
