using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;

using SmartTask.Core.IRepositories;
using BranchDepartment = SmartTask.Core.Models.BranchDepartment;

namespace SmartTask.DataAccess.Repositories
{
    public class BranchDepartmentRepository : IBranchDepartmentRepository
    {
        private readonly SmartTaskContext _context;

        public BranchDepartmentRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BranchDepartment>> GetAllAsync()
        {
            return await _context.BranchDepartments
                .Include(bd => bd.Branch)
                .Include(bd => bd.Department)
                .ToListAsync();
        }

        public async Task<BranchDepartment> GetByIdsAsync(int branchId, int departmentId)
        {
            return await _context.BranchDepartments
                .Include(bd => bd.Branch)
                .Include(bd => bd.Department)
                .FirstOrDefaultAsync(bd => bd.BranchId == branchId && bd.DepartmentId == departmentId);
        }

        public async Task<IEnumerable<BranchDepartment>> GetByBranchIdAsync(int branchId)
        {
            return await _context.BranchDepartments
                .Include(bd => bd.Department)
                .Where(bd => bd.BranchId == branchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BranchDepartment>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.BranchDepartments
                .Include(bd => bd.Branch)
                .Where(bd => bd.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task AddAsync(BranchDepartment branchDepartment)
        {
            _context.BranchDepartments.Add(branchDepartment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int branchId, int departmentId)
        {
            var entity = await _context.BranchDepartments
                .FirstOrDefaultAsync(bd => bd.BranchId == branchId && bd.DepartmentId == departmentId);
            
            if (entity != null)
            {
                _context.BranchDepartments.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int branchId, int departmentId)
        {
            return await _context.BranchDepartments
                .AnyAsync(bd => bd.BranchId == branchId && bd.DepartmentId == departmentId);
        }
    }
}