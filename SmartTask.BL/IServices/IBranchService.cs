using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartTask.Bl.Helpers;
using SmartTask.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.Bl.IServices
{
    public interface IBranchService
    {

        Task<Branch> GetBranchAsync(int id);
        PaginatedList<Branch> GetAllBranchAsync(int page, int pageSize);
        Task<IEnumerable<Branch>> GetAllAsync();
        Task<Branch> AddAsync(Branch branch);

        Task UpdateAsync(Branch branch);

        Task DeleteAsync(int id);

        Task<PaginatedList<Branch>> GetFiltered(string searchString, string? managerId, int page, int pageSize);

        Task<Branch> GetBranchWithDetailsAsync(int id);

    }
}
