using System.Collections.Generic;
using System.Threading.Tasks;
using Department = SmartTask.Core.Models.Department;

namespace SmartTask.Core.IRepositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> GetByIdAsync(int id);
        Task<Department> GetWithDetailsAsync(int id);
        Task<IEnumerable<Department>> GetByManagerIdAsync(int managerId);
        Task<IEnumerable<Department>> GetByBranchIdAsync(int branchId);
        Task<Department> AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
