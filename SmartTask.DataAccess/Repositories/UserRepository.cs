using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;


namespace SmartTask.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SmartTaskContext _context;

        public UserRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u)
                .Include(u => u.Department)
                .ToListAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(String id)
        {
            return await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser> GetWithDetailsAsync(String id)
        {
            return await _context.Users
                .Include(u => u.Department)
                .Include(u => u.ManagedBranches)
                .Include(u => u.ManagedDepartments)
                .Include(u => u.ProjectMemberships)
                    .ThenInclude(pm => pm.Project)
                .Include(u => u.ProjectMemberships)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Users
                .Where(u => u.DepartmentId == departmentId)
                .ToListAsync();
        }

        //public async Task<IEnumerable<ApplicationUser>> GetByRoleIdAsync(int roleId)
        //{
        //    return await _context.Users
        //        .Include(u => u.Department)
        //        .Where(u => u.RoleId == roleId)
        //        .ToListAsync();
        //}

        //public async Task<User> AddAsync(User user)
        //{
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    return user;
        //}

        //public async Task UpdateAsync(User user)
        //{
        //    _context.Entry(user).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteAsync(int id)
        //{
        //    var user = await _context.Users.FindAsync(id);
        //    if (user != null)
        //    {
        //        _context.Users.Remove(user);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        //public async Task<bool> ExistsAsync(string id)
        //{
        //    return await _context.Users.AnyAsync(u => u.Id == id);
        //}

        //public async Task<bool> EmailExistsAsync(string email)
        //{
        //    return await _context.Users.AnyAsync(u => u.Email == email);
        //}
    }
}