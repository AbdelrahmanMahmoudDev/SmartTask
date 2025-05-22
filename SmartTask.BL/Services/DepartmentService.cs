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
            IPaginatedService<Department> paginatedService,UserManager<ApplicationUser> _userManager)
        {
            _departmentRepository = departmentRepository;
            _paginatedService = paginatedService;
            _userManager = _userManager;
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
            {
                throw new InvalidOperationException("Department not found");
            }

            existingDepartment.Name = department.Name;
            existingDepartment.ManagerId = department.ManagerId;

            var currentUserIds = existingDepartment.Users.Select(u => u.Id).ToList();
            var newUserIds = department.Users.Select(u => u.Id).ToList();

            var usersToAddIds = newUserIds.Except(currentUserIds).ToList();
            var usersToRemoveIds = currentUserIds.Except(newUserIds).ToList();

            foreach (var userId in usersToAddIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    existingDepartment.Users.Add(user);
                }
            }

            foreach (var userId in usersToRemoveIds)
            {
                var user = existingDepartment.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    existingDepartment.Users.Remove(user);
                }
            }

            await _departmentRepository.UpdateAsync(existingDepartment);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
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
