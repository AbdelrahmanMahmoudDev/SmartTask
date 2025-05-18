using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTask.Core.Models;
using Task = System.Threading.Tasks.Task;
namespace SmartTask.BL.IServices
{
    public interface IDashboardService
    {
        Task<UserDashboardPreference> GetUserDashboardSettingsAsync(string userId);
        Task SaveUserDashboardSettingsAsync(string userId, UserDashboardPreference settings);
        Task<UserDashboardPreference> GetUserPreferenceAsync(string userId);
        Task UpdateUserPreferenceAsync(UserDashboardPreference preference);
    }
}
