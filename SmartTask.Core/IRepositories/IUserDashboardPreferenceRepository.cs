using SmartTask.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.Core.IRepositories
{
    public interface IUserDashboardPreferenceRepository
    {
        Task<UserDashboardPreference> GetByUserIdAsync(string userId);
        Task<UserDashboardPreference> GetByUserIdWithUserAsync(string userId); 
        Task<UserDashboardPreference> GetByIdAsync(int id);
        Task<UserDashboardPreference> AddAsync(UserDashboardPreference preference);
        Task UpdateAsync(UserDashboardPreference preference);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string userId);
    }
}
