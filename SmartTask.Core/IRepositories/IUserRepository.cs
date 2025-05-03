using SmartTask.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SmartTask.Core.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser> GetByIdAsync(String id);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<ApplicationUser> GetWithDetailsAsync(String id);
        Task<IEnumerable<ApplicationUser>> GetByDepartmentIdAsync(int departmentId);
        //Task<IEnumerable<ApplicationUser>> GetByRoleIdAsync(int roleId);
        //Task<ApplicationUser> AddAsync(ApplicationUser user);
        //Task UpdateAsync(ApplicationUser user);
        //Task DeleteAsync(int id);
        //Task<bool> ExistsAsync(int id);
        //Task<bool> EmailExistsAsync(string email);
    }
}