using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task= System.Threading.Tasks.Task;
using SmartTask.Bl.Helpers;
using SmartTask.Core.Models;


namespace SmartTask.BL.IServices
{
    public interface IDepartmentService
    {
        Task<PaginatedList<Department>> GetFilteredDepartments(string searchString, int page, int pageSize);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(int id);
        Task<Department> AddDepartmentAsync(Department department);
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentWithDetailsAsync(int id);

    }
}
