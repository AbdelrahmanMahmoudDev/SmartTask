using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Graph.Models;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.BL.Services
{
    public class DashboardService:IDashboardService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserDashboardPreferenceRepository _preferenceRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardService(IHttpContextAccessor httpContextAccessor, IUserDashboardPreferenceRepository preferenceRepository, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _preferenceRepository = preferenceRepository;
            _userManager = userManager;
        }

        public async Task<UserDashboardPreference> GetUserDashboardSettingsAsync(string userId)
        {
            // First try to get from database with user information
            var preference = await _preferenceRepository.GetByUserIdWithUserAsync(userId);

            if (preference != null)
            {
                // Update last login date if it's a new login
                if (!preference.LastLoginDate.HasValue ||
                    (DateTime.Now - preference.LastLoginDate.Value).TotalHours > 1)
                {
                    preference.LastLoginDate = DateTime.Now;
                    await _preferenceRepository.UpdateAsync(preference);
                }
                return preference;
            }

            // Try to get from session before creating new
            var session = _httpContextAccessor.HttpContext?.Session;
            var sessionSettings = session?.GetString($"dashboard_{userId}");

            if (!string.IsNullOrEmpty(sessionSettings))
            {
                var sessionPreference = JsonSerializer.Deserialize<UserDashboardPreference>(sessionSettings);
                if (sessionPreference != null)
                {
                    // Load user information for session preference
                    sessionPreference.User = await _userManager.FindByIdAsync(userId);
                    return sessionPreference;
                }
            }

            // Create default preference with user information
            var user = await _userManager.FindByIdAsync(userId);
            preference = new UserDashboardPreference
            {
                UserId = userId,
                ShowRecentProjects = true,
                ShowProjectStatus = true,
                ShowUpcomingTasks = true,
                ShowActivityChart = true,
                RecentProjectsCount = 5,
                PreferredView = "grid",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                LastLoginDate = DateTime.Now,
                User = user
            };

            // Save the default preference to the database for next time
            await _preferenceRepository.AddAsync(preference);

            // Also save to session
            var preferenceForSession = new UserDashboardPreference
            {
                UserId = preference.UserId,
                ShowRecentProjects = preference.ShowRecentProjects,
                ShowProjectStatus = preference.ShowProjectStatus,
                ShowUpcomingTasks = preference.ShowUpcomingTasks,
                ShowActivityChart = preference.ShowActivityChart,
                RecentProjectsCount = preference.RecentProjectsCount,
                PreferredView = preference.PreferredView,
                CreatedAt = preference.CreatedAt,
                UpdatedAt = preference.UpdatedAt,
                LastLoginDate = preference.LastLoginDate
            };
            session?.SetString($"dashboard_{userId}", JsonSerializer.Serialize(preferenceForSession));

            return preference;
        }

        public async Task SaveUserDashboardSettingsAsync(string userId, UserDashboardPreference settings)
        {
            settings.UserId = userId;

            var existingPreference = await _preferenceRepository.GetByUserIdAsync(userId);

            if (existingPreference == null)
            {
                // Add new preference
                settings.CreatedAt = DateTime.Now;
                settings.UpdatedAt = DateTime.Now;
                await _preferenceRepository.AddAsync(settings);
            }
            else
            {
                // Update existing preference
                existingPreference.ShowRecentProjects = settings.ShowRecentProjects;
                existingPreference.ShowProjectStatus = settings.ShowProjectStatus;
                existingPreference.ShowUpcomingTasks = settings.ShowUpcomingTasks;
                existingPreference.ShowActivityChart = settings.ShowActivityChart;
                existingPreference.RecentProjectsCount = settings.RecentProjectsCount;
                existingPreference.PreferredView = settings.PreferredView;
                existingPreference.UpdatedAt = DateTime.Now;

                await _preferenceRepository.UpdateAsync(existingPreference);
                await _preferenceRepository.UpdateAsync(settings);

            }

            // Also save to session for quick access (exclude navigation properties)
            var session = _httpContextAccessor.HttpContext?.Session;
            var preferenceForSession = new UserDashboardPreference
            {
                UserId = settings.UserId,
                ShowRecentProjects = settings.ShowRecentProjects,
                ShowProjectStatus = settings.ShowProjectStatus,
                ShowUpcomingTasks = settings.ShowUpcomingTasks,
                ShowActivityChart = settings.ShowActivityChart,
                RecentProjectsCount = settings.RecentProjectsCount,
                PreferredView = settings.PreferredView,
                CreatedAt = settings.CreatedAt,
                UpdatedAt = settings.UpdatedAt,
                LastLoginDate = settings.LastLoginDate
            };
            session?.SetString($"dashboard_{userId}", JsonSerializer.Serialize(preferenceForSession));
        }

        public async Task<UserDashboardPreference> GetUserPreferenceAsync(string userId)
        {
            var preference = await _preferenceRepository.GetByUserIdWithUserAsync(userId);

            // If preference doesn't exist, create a default one
            if (preference == null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                preference = new UserDashboardPreference
                {
                    UserId = userId,
                    ShowRecentProjects = true,
                    ShowProjectStatus = true,
                    ShowUpcomingTasks = true,
                    ShowActivityChart = true,
                    RecentProjectsCount = 5,
                    PreferredView = "grid",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    User = user
                };

                // Save the default preference
                await _preferenceRepository.AddAsync(preference);
            }

            return preference;
        }

        public async Task UpdateUserPreferenceAsync(UserDashboardPreference preference)
        {
            preference.UpdatedAt = DateTime.Now;

            try
            {
                await _preferenceRepository.UpdateAsync(preference);
            }
            catch (Exception)
            {
                // If update fails (e.g., preference doesn't exist), create it
                if (!await _preferenceRepository.ExistsAsync(preference.UserId))
                {
                    preference.CreatedAt = DateTime.Now;
                    await _preferenceRepository.AddAsync(preference);
                }
                else
                {
                    // Re-throw if it's a different error
                    throw;
                }
            }

            // Update session as well (exclude navigation properties)
            var session = _httpContextAccessor.HttpContext?.Session;
            var preferenceForSession = new UserDashboardPreference
            {
                UserId = preference.UserId,
                ShowRecentProjects = preference.ShowRecentProjects,
                ShowProjectStatus = preference.ShowProjectStatus,
                ShowUpcomingTasks = preference.ShowUpcomingTasks,
                ShowActivityChart = preference.ShowActivityChart,
                RecentProjectsCount = preference.RecentProjectsCount,
                PreferredView = preference.PreferredView,
                CreatedAt = preference.CreatedAt,
                UpdatedAt = preference.UpdatedAt,
                LastLoginDate = preference.LastLoginDate
            };
            session?.SetString($"dashboard_{preference.UserId}", JsonSerializer.Serialize(preferenceForSession));
        }
    }
}

