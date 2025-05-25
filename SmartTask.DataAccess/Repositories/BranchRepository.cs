using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using Branch = SmartTask.Core.Models.Branch;

namespace SmartTask.DataAccess.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly SmartTaskContext _context;

        public BranchRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetAllAsync()
        {
            return await _context.Branches
                .Include(b => b.Manager)
                .ToListAsync();
        }

        public async Task<Branch> GetByIdAsync(int id)
        {
            return await _context.Branches
                .Include(b => b.Manager)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Branch> GetWithDetailsAsync(int id)
        {
            return await _context.Branches
                .Include(b => b.Manager)
                .Include(b => b.BranchDepartments)
                    .ThenInclude(bd => bd.Department)
                    .Include(b => b.Users)
                    .Include(b => b.Projects)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Branch>> GetByManagerIdAsync(string managerId)
        {
            return await _context.Branches
                .Where(b => b.ManagerId == managerId)
                .ToListAsync();
        }

        public async Task<Branch> AddAsync(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task UpdateAsync(Branch branch)
        {
            _context.Entry(branch).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Branches.AnyAsync(b => b.Id == id);
        }

        public IQueryable<Branch> GetQueryable()
        {
            return _context.Branches.Include(b => b.Manager)
                                   .Include(b => b.BranchDepartments)
                                   .ThenInclude(bd => bd.Department)
                                   .AsQueryable();
        }

        public async Task<Branch> GetBranchWithDepartmentsAsync(int id)
        {
            return await _context.Branches.Include(b => b.BranchDepartments).FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}