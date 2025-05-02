using System.Collections.Generic;
using System.Threading.Tasks;


using RolePermission = SmartTask.Core.Models.RolePermission;
namespace SmartTask.Core.IRepositories
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RolePermission>> GetAllAsync();
        Task<RolePermission> GetByIdsAsync(int roleId, int permissionId);
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);
        Task AddAsync(RolePermission rolePermission);
        Task DeleteAsync(int roleId, int permissionId);
        Task<bool> ExistsAsync(int roleId, int permissionId);
    }
}