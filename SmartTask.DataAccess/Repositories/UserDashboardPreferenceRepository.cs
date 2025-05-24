using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.DataAccess.Repositories
{
    public class UserDashboardPreferenceRepository : IUserDashboardPreferenceRepository
    {
        private readonly SmartTaskContext _context;

        public UserDashboardPreferenceRepository(SmartTaskContext context)
        {
            _context = context;
        }

        // Get preferences by user ID
        public async Task<UserDashboardPreference> GetByUserIdAsync(string userId)
        {
            return await _context.UserDashboardPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }


        // Get preferences by user ID and include User data
        public async Task<UserDashboardPreference> GetByUserIdWithUserAsync(string userId)
        {
            return await _context.UserDashboardPreferences
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }


        // Get preferences by preference ID
        public async Task<UserDashboardPreference> GetByIdAsync(int id)
        {

            return await _context.UserDashboardPreferences.FindAsync(id);
        }


        // Add new preference if not exists, otherwise update existing one
        public async Task<UserDashboardPreference> AddAsync(UserDashboardPreference preference)
        {
            var existingPreference = await GetByUserIdAsync(preference.UserId);
            if (existingPreference != null)
            {
                // Preference already exists → update it instead
                return await UpdateAndReturnAsync(preference);
            }

            // Add new preference
            _context.UserDashboardPreferences.Add(preference);
            await _context.SaveChangesAsync();
            return preference;
        }


        // Update existing preference, or add it if not found
        public async Task UpdateAsync(UserDashboardPreference preference)
        {
            var existingPreference = await _context.UserDashboardPreferences
                                           .FirstOrDefaultAsync(p => p.UserId == preference.UserId);
            if (existingPreference == null)
            {
                // If not found → just add it
                _context.UserDashboardPreferences.Add(preference);
                await _context.SaveChangesAsync();
                return;
            }

            // Update fields
            existingPreference.ShowRecentProjects = preference.ShowRecentProjects;
            existingPreference.ShowProjectStatus = preference.ShowProjectStatus;
            existingPreference.ShowUpcomingTasks = preference.ShowUpcomingTasks;
            existingPreference.ShowMyTasks = preference.ShowMyTasks;
            existingPreference.ShowTasksOverview = preference.ShowTasksOverview;
            existingPreference.RecentProjectsCount = preference.RecentProjectsCount;
            existingPreference.PreferredView = preference.PreferredView;
            existingPreference.UpdatedAt = DateTime.Now;

            // Update LastLoginDate if it has a value
            if (preference.LastLoginDate.HasValue)
            {
                existingPreference.LastLoginDate = preference.LastLoginDate;
            }

            await _context.SaveChangesAsync();
        }


        // Helper method to update and return the preference
        private async Task<UserDashboardPreference> UpdateAndReturnAsync(UserDashboardPreference preference)
        {
            var existingPreference = await _context.UserDashboardPreferences
                .FirstOrDefaultAsync(p => p.UserId == preference.UserId);

            if (existingPreference != null)
            {
                existingPreference.ShowRecentProjects = preference.ShowRecentProjects;
                existingPreference.ShowProjectStatus = preference.ShowProjectStatus;
                existingPreference.ShowUpcomingTasks = preference.ShowUpcomingTasks;
                existingPreference.ShowMyTasks = preference.ShowMyTasks;
                existingPreference.ShowTasksOverview = preference.ShowTasksOverview;
                existingPreference.RecentProjectsCount = preference.RecentProjectsCount;
                existingPreference.PreferredView = preference.PreferredView;
                existingPreference.UpdatedAt = DateTime.Now;

                if (preference.LastLoginDate.HasValue)
                {
                    existingPreference.LastLoginDate = preference.LastLoginDate;
                }

                await _context.SaveChangesAsync();
                return existingPreference;
            }

            // If not found → add a new preference
            _context.UserDashboardPreferences.Add(preference);
            await _context.SaveChangesAsync();
            return preference;
        }

        public async Task DeleteAsync(int id)
        {
            var preference = await _context.UserDashboardPreferences.FindAsync(id);
            if (preference != null)
            {
                _context.UserDashboardPreferences.Remove(preference);
                await _context.SaveChangesAsync();
            }
        }

        // Check if preference exists for a specific user
        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.UserDashboardPreferences
                .AnyAsync(p => p.UserId == userId);
        }
    }
}
