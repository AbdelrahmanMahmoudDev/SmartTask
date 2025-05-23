using Microsoft.AspNetCore.Identity;
using SmartTask.Bl.Helpers;
using SmartTask.Bl.Services;
using SmartTask.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<PaginatedList<ApplicationUser>> GetAll(int page, int pageSize);
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<bool> UpdateAsync(ApplicationUser user);
        Task<bool> DeleteAsync(string id);

        Task<PaginatedList<ApplicationUser>> GetFilteredAsync( Expression<Func<ApplicationUser, bool>>? filter,
            int page,
            int pageSize);

        Task<bool> AssignUserToDepartmentAsync(string userId, int departmentId);

        //Task< PaginatedList<ApplicationUser>> GetFilteredAsync(string searchString, int page, int pageSize);

        //Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);

    }
}
