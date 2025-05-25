using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Task= System.Threading.Tasks.Task;
using SmartTask.Bl.Helpers;
using SmartTask.Bl.IServices;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SmartTask.BL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPaginatedService<Department> _paginatedService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IPaginatedService<Department> paginatedService,UserManager<ApplicationUser> userManager)
        {
            _departmentRepository = departmentRepository;
            _paginatedService = paginatedService;
            _userManager = userManager;
        }

        public async Task<PaginatedList<Department> >GetFilteredDepartments(string searchString, int page, int pageSize)
        {
            var query = _departmentRepository.GetQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.Name.Contains(searchString));
            }

            return await PaginatedList<Department>.CreateAsync(query, page, pageSize);
        }

        public async Task<Department> AddDepartmentAsync(Department department)
        {
            return await _departmentRepository.AddAsync(department);
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            var existingDepartment = await _departmentRepository.GetQueryable()
                .Include(d => d.BranchDepartments)
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == department.Id);

            if (existingDepartment == null)
                throw new InvalidOperationException("Department not found");

            existingDepartment.Name = department.Name;
            existingDepartment.ManagerId = department.ManagerId;

            var newBranchIds = department.BranchDepartments.Select(bd => bd.BranchId).ToList();
            var existingBranchIds = existingDepartment.BranchDepartments.Select(bd => bd.BranchId).ToList();

            foreach (var bd in existingDepartment.BranchDepartments.ToList())
            {
                if (!newBranchIds.Contains(bd.BranchId))
                    existingDepartment.BranchDepartments.Remove(bd);
            }
            foreach (var branchId in newBranchIds)
            {
                if (!existingBranchIds.Contains(branchId))
                    existingDepartment.BranchDepartments.Add(new BranchDepartment
                    {
                        BranchId = branchId,
                        DepartmentId = department.Id
                    });
            }

            await _departmentRepository.UpdateAsync(existingDepartment);

            existingDepartment = await _departmentRepository.GetQueryable()
                .Include(d => d.BranchDepartments)
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == department.Id);

            var newUserIds = department.Users.Select(u => u.Id).ToList();
            var currentUserIds = existingDepartment.Users.Select(u => u.Id).ToList();

            foreach (var userId in currentUserIds.Except(newUserIds).ToList())
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.DepartmentId = null;
                    await _userManager.UpdateAsync(user);
                    existingDepartment.Users.Remove(user);
                }
            }

            foreach (var userId in newUserIds.Except(currentUserIds).ToList())
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (user.BranchId.HasValue && !existingDepartment.BranchDepartments.Any(bd => bd.BranchId == user.BranchId.Value))
                        continue;

                    user.DepartmentId = department.Id;
                    await _userManager.UpdateAsync(user);
                    existingDepartment.Users.Add(user);
                }
            }

            await _departmentRepository.UpdateAsync(existingDepartment);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            if (department == null)
                throw new InvalidOperationException("Department not found");

            await _departmentRepository.DeleteAsync(id);
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            //return await _departmentRepository.GetByIdAsync(id);
            return await _departmentRepository.GetQueryable()
                                                .Include(d => d.Users)
                                                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department> GetDepartmentWithDetailsAsync(int id)
        {
            return await _departmentRepository.GetWithDetailsAsync(id);
        }
    }
}
