using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SmartTask.Core.Models.Enums;
using SmartTask.Core.Models;
using Task = System.Threading.Tasks.Task;
using SmartTask.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using SmartTask.DataAccess.Data;

namespace SmartTask.BL.Services
{
    public class UserColumnPreferenceService : IUserColumnPreferenceService
    {
        private readonly IUserColumnPreferenceRepository _userColumnPreferenceRepository;
        private readonly SmartTaskContext _context;

        public UserColumnPreferenceService(IUserColumnPreferenceRepository userColumnPreferenceRepository, SmartTaskContext context)
        {
            _userColumnPreferenceRepository = userColumnPreferenceRepository;
            _context = context;
        }

        public async Task<List<UserColumnPreference>> GetUserColumns(string userId)
        {
            var preferences = await _userColumnPreferenceRepository.GetByUserId(userId);

            await _context.SaveChangesAsync();
            Console.WriteLine("Changes saved to database successfully.");

            preferences = await _context.UserColumnPreferences
                .Where(u => u.UserId == userId)
                .ToListAsync();

            Console.WriteLine("Reloaded preferences from database:");
            foreach (var preference in preferences)
            {
                Console.WriteLine($"Column ID: {preference.Id}, Order: {preference.Order}");
            }
           

            if (preferences == null || !preferences.Any())
            {
                Console.WriteLine($"No preferences found for user ID '{userId}'.");
                return new List<UserColumnPreference>();
            }

            return preferences;
        }

        public async Task<bool> UpdateColumnOrder(string userId, List<ColumnOrderUpdate> columnOrder)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Console.WriteLine($"Updating order for user: {userId}");

                var preferences = await _context.UserColumnPreferences
                    .Where(u => u.UserId == userId)
                    .ToListAsync();

                Console.WriteLine($"Found {preferences.Count} preferences for user: {userId}");

                foreach (var update in columnOrder)
                {
                    var preference = preferences.FirstOrDefault(p => p.Id == update.ColumnId);
                    if (preference == null)
                    {
                        Console.WriteLine($"❌ Column ID {update.ColumnId} not found for user {userId}");
                        return false;
                    }

                    Console.WriteLine($"Updating column ID {preference.Id} (Status: {preference.Status}) to order {update.Order}");
                    preference.Order = update.Order;
                }

                Console.WriteLine("Saving changes to database...");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                Console.WriteLine("Transaction committed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"❌ Error in UpdateColumnOrder: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDisplayName(string userId, Status status, string displayName)
        {
            var preference = await _userColumnPreferenceRepository.GetByUserIdAndStatus(userId, status);
            if (preference == null) return false;

            preference.DisplayName = displayName;
            await _userColumnPreferenceRepository.UpdateAsync(preference);
            return true;
        }

        public async Task InitializeDefaultColumns(string userId)
        {
            if (await _context.UserColumnPreferences.AnyAsync(u => u.UserId == userId))
                return;

            var defaultColumns = new List<UserColumnPreference>
            {
                new() { UserId = userId, Status = Status.Todo, DisplayName = "Todo", Order = 0 },
                new() { UserId = userId, Status = Status.InProgress, DisplayName = "In Progress", Order = 1 },
                new() { UserId = userId, Status = Status.Done, DisplayName = "Done", Order = 2 }
            };

            await _context.UserColumnPreferences.AddRangeAsync(defaultColumns);
            await _context.SaveChangesAsync();
        }
    }

}
