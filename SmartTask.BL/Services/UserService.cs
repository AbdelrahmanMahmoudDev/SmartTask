using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTask.Bl.Helpers;
using SmartTask.Bl.IServices;
using SmartTask.Bl.Services;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
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
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPaginatedService<ApplicationUser> _paginatedService;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IPaginatedService<ApplicationUser> paginatedService,
            IDepartmentRepository departmentRepository)
        {
            _userManager = userManager;
            _paginatedService = paginatedService;
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users
                .Include(u => u.Department)
                .ToListAsync();
        }
        public async Task< PaginatedList<ApplicationUser>> GetAll(int page, int pageSize)
        {
            return await _paginatedService.GetFilteredAsync(null, page, pageSize);
        }
        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _userManager.Users.Include(u => u.Department).FirstOrDefaultAsync(u => u.Id == id);
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

        public async Task<PaginatedList<ApplicationUser>> GetFilteredAsync(string searchString, int page, int pageSize)
        {
            var query = _userManager.Users.Include(u => u.Department).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(u => u.FullName.Contains(searchString) || u.Email.Contains(searchString));
            }

            return await PaginatedList<ApplicationUser>.CreateAsync(query, page, pageSize);
        }

        public async Task<bool> AssignUserToDepartmentAsync(string userId, int departmentId)
        {
            var user = _userManager.Users.Include(u => u.Department).FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            if (user.DepartmentId != null) return false;

            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null) return false;

            user.DepartmentId = department.Id;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
