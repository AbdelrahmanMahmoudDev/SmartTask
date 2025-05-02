using System.Collections.Generic;
using System.Threading.Tasks;
using Permission = SmartTask.Core.Models.Permission;


namespace SmartTask.Core.IRepositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission> GetByIdAsync(int id);
        Task<Permission> AddAsync(Permission permission);
        Task UpdateAsync(Permission permission);
        Task DeleteAsync(int id);
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId);
        Task<bool> ExistsAsync(int id);
    }
}