using System.Collections.Generic;
using System.Threading.Tasks;
using BranchDepartment = SmartTask.Core.Models.BranchDepartment;

namespace SmartTask.Core.IRepositories
{
    public interface IBranchDepartmentRepository
    {
        Task<IEnumerable<BranchDepartment>> GetAllAsync();
        Task<BranchDepartment> GetByIdsAsync(int branchId, int departmentId);
        Task<IEnumerable<BranchDepartment>> GetByBranchIdAsync(int branchId);
        Task<IEnumerable<BranchDepartment>> GetByDepartmentIdAsync(int departmentId);
        Task AddAsync(BranchDepartment branchDepartment);
        Task DeleteAsync(int branchId, int departmentId);
        Task<bool> ExistsAsync(int branchId, int departmentId);
    }
}
