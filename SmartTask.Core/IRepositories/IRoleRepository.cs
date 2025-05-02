using System.Collections.Generic;
using System.Threading.Tasks;
using Role = SmartTask.Core.Models.Role;

namespace SmartTask.Core.IRepositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(int id);
        Task<Role> AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(int id);
        Task<IEnumerable<Role>> GetRolesWithPermissionsAsync();
        Task<bool> ExistsAsync(int id);
    }
}