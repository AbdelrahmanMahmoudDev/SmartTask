using System.Collections.Generic;
using System.Threading.Tasks;

using ProjectRolePermission = SmartTask.Core.Models.ProjectRolePermission;

namespace SmartTask.Core.IRepositories
{
    public interface IProjectRolePermissionRepository
    {
        Task<IEnumerable<ProjectRolePermission>> GetAllAsync();
        Task<ProjectRolePermission> GetByIdsAsync(int projectRoleId, int permissionId);
        Task<IEnumerable<ProjectRolePermission>> GetByProjectRoleIdAsync(int projectRoleId);
        Task<IEnumerable<ProjectRolePermission>> GetByPermissionIdAsync(int permissionId);
        Task AddAsync(ProjectRolePermission projectRolePermission);
        Task DeleteAsync(int projectRoleId, int permissionId);
        Task<bool> ExistsAsync(int projectRoleId, int permissionId);
    }
}