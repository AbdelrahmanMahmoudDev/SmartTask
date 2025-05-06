using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SmartTask.Bl.Helpers;
using SmartTask.Bl.IServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Repositories;
using Task = System.Threading.Tasks.Task;
namespace SmartTask.Bl.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository branchRepository;
        private readonly IPaginatedService<Branch> paginatedService;

        public BranchService(IBranchRepository branchRepository, IPaginatedService<Branch> paginatedService)
        {
            this.branchRepository = branchRepository;
            this.paginatedService = paginatedService;
        }

        public PaginatedList<Branch> GetAllBranchAsync(int page, int pageSize)
        {
            var query = branchRepository.GetQueryable();
            return PaginatedList<Branch>.Create(query,page, pageSize);
        }

        public async Task<Branch> GetBranchAsync(int id)
        {
            return await branchRepository.GetByIdAsync(id);
        }
        public async Task<Branch> AddAsync(Branch branch)
        {
            return await branchRepository.AddAsync(branch);
        }

        public async Task DeleteAsync(int id)
        {
            await branchRepository.DeleteAsync(id);
        }


        public async Task UpdateAsync(Branch branch)
        {
           await branchRepository.UpdateAsync(branch);
        }


        public PaginatedList<Branch> GetFiltered(string searchString, string? managerId, int page, int pageSize)
        {
            Expression<Func<Branch, bool>> filter = b =>
                (string.IsNullOrEmpty(searchString) || b.Name.Contains(searchString)) &&
                (managerId != null || b.ManagerId == managerId);

            return paginatedService.GetFiltered(filter, page, pageSize);
        }
    }
}
