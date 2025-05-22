using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public BranchService(IBranchRepository branchRepository, IPaginatedService<Branch> paginatedService,
            IDepartmentRepository departmentRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            this.branchRepository = branchRepository;
            this.paginatedService = paginatedService;
            _userManager = userManager;
        }

        public async Task<PaginatedList<Branch>> GetAllBranchAsync(int page, int pageSize)
        {
            var query = branchRepository.GetQueryable();
            return await PaginatedList<Branch>.CreateAsync(query,page, pageSize);
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
            var existingBranch = await branchRepository.GetWithDetailsAsync(branch.Id);
            if (existingBranch == null)
            {
                throw new Exception("Branch not found");
            }

            existingBranch.Name = branch.Name;
            existingBranch.ManagerId = branch.ManagerId;

            existingBranch.BranchDepartments.Clear(); 
            if (branch.BranchDepartments != null && branch.BranchDepartments.Any())
            {
                foreach (var bd in branch.BranchDepartments)
                {
                    existingBranch.BranchDepartments.Add(new BranchDepartment
                    {
                        BranchId = branch.Id,
                        DepartmentId = bd.DepartmentId
                    });
                }
            }

            await branchRepository.UpdateAsync(existingBranch);
        }


        //public async Task<PaginatedList<Branch>> GetFiltered(string searchString, string? managerId, int page, int pageSize)
        //{
        //    var query = branchRepository.GetQueryable()
        //        .Include(b => b.Manager) 
        //        .Include(b => b.BranchDepartments) 
        //        .ThenInclude(bd => bd.Department);

        //    Expression<Func<Branch, bool>> filter = b =>
        //        (string.IsNullOrEmpty(searchString) || b.Name.Contains(searchString)) &&
        //        (string.IsNullOrEmpty(managerId) || b.ManagerId == managerId);

        //    return await paginatedService.GetFiltered(filter, page, pageSize);
        //}

        public async Task<PaginatedList<Branch>> GetFiltered(string searchString, string? managerId, int page, int pageSize)
        {
            var query = branchRepository.GetQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(managerId))
            {
                query = query.Where(b => b.ManagerId == managerId);
            }

            return await paginatedService.GetFilteredAsync(query, page, pageSize);
        }


        public async Task<Branch> GetBranchWithDetailsAsync(int id)
        {
            return await branchRepository.GetWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            return  await branchRepository.GetAllAsync();
        }
    }
}
