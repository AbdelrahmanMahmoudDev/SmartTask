using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using Department = SmartTask.Core.Models.Department;


namespace SmartTask.DataAccess.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly SmartTaskContext _context;

        public DepartmentRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .ToListAsync();
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department> GetWithDetailsAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.BranchDepartments)
                    .ThenInclude(bd => bd.Branch)
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Department>> GetByManagerIdAsync(string managerId)
        {
            return await _context.Departments
                .Where(d => d.ManagerId == managerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetByBranchIdAsync(int branchId)
        {
            return await _context.BranchDepartments
                .Where(bd => bd.BranchId == branchId)
                .Select(bd => bd.Department)
                .Include(d => d.Manager)
                .ToListAsync();
        }

        public async Task<Department> AddAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Departments.AnyAsync(d => d.Id == id);
        }
    }
}