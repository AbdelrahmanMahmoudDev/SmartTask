using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTask.Bl.Helpers;
using SmartTask.Bl.IServices;
using SmartTask.Bl.Services;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaginatedService<ApplicationUser> _paginatedService;

        public UserService(UserManager<ApplicationUser> userManager,IPaginatedService<ApplicationUser> paginatedService)
        {
            _userManager = userManager;
            _paginatedService = paginatedService;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users
                .Include(u => u.Department)
                .ToListAsync();
        }
        public async Task< PaginatedList<ApplicationUser>> GetAll(int page, int pageSize)
        {
            return await _paginatedService.GetFiltered(null, page, pageSize);
        }
        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateAsync(ApplicationUser updatedUser)
        {
            var user = await _userManager.FindByIdAsync(updatedUser.Id);
            if (user == null) return false;

            user.FullName = updatedUser.FullName;
            user.DepartmentId = updatedUser.DepartmentId;
            user.updatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<PaginatedList<ApplicationUser>> GetFilteredAsync(Expression<Func<ApplicationUser, bool>>? filter,
            int page , int pageSize)
        {
            var query = _userManager.Users
                .Include(u => u.Department)
                .AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await System.Threading.Tasks.Task.FromResult(PaginatedList<ApplicationUser>.Create(query, page, pageSize));
        }
    }
}
