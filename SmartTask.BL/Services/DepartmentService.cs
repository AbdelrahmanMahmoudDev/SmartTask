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

namespace SmartTask.BL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPaginatedService<Department> _paginatedService;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IPaginatedService<Department> paginatedService)
        {
            _departmentRepository = departmentRepository;
            _paginatedService = paginatedService;
        }

        public async Task<PaginatedList<Department> >GetFilteredDepartments(string searchString, int page, int pageSize)
        {
            var query = _departmentRepository.GetQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.Name.Contains(searchString));
            }

            return PaginatedList<Department>.Create(query, page, pageSize);
        }

        public async Task<Department> AddDepartmentAsync(Department department)
        {
            return await _departmentRepository.AddAsync(department);
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            await _departmentRepository.UpdateAsync(department);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            await _departmentRepository.DeleteAsync(id);
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
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
