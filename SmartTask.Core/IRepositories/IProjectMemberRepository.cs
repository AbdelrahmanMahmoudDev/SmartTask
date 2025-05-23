using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMember = SmartTask.Core.Models.ProjectMember;

namespace SmartTask.Core.IRepositories
{
    public interface IProjectMemberRepository
    {
        Task<IEnumerable<ProjectMember>> GetAllAsync();
        Task<ProjectMember> GetByIdsAsync(int projectId, string userId);
        Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(int projectId);
        Task<IEnumerable<ProjectMember>> GetByUserIdAsync(string userId);
        //Task<IEnumerable<ProjectMember>> GetByProjectRoleIdAsync(int projectRoleId);
        Task AddAsync(ProjectMember projectMember);
        Task UpdateAsync(ProjectMember projectMember);
        Task DeleteAsync(int projectId, string userId);
        Task<bool> ExistsAsync(int projectId, string userId);
    }
}