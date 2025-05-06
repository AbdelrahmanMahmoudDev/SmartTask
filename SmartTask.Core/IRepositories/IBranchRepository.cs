using System.Collections.Generic;
using System.Threading.Tasks;
using Branch = SmartTask.Core.Models.Branch;

namespace SmartTask.Core.IRepositories
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Branch>> GetAllAsync();
        Task<Branch> GetByIdAsync(int id);
        Task<Branch> GetWithDetailsAsync(int id);
        Task<IEnumerable<Branch>> GetByManagerIdAsync(string managerId);
        Task<Branch> AddAsync(Branch branch);
        Task UpdateAsync(Branch branch);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        IQueryable<Branch> GetQueryable();
    }
}
