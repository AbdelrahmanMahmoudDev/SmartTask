using System.Collections.Generic;
using System.Threading.Tasks;
using User = SmartTask.Core.Models.User;


namespace SmartTask.Core.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetWithDetailsAsync(int id);
        Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}