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
            return await _userColumnPreferenceRepository.GetByUserId(userId);
        }

        public async Task<bool> UpdateColumnOrder(string userId, List<ColumnOrderUpdate> columnOrder)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var preferences = await _context.UserColumnPreferences
                    .Where(u => u.UserId == userId)
                    .ToListAsync();

                foreach (var update in columnOrder)
                {
                    var preference = preferences.FirstOrDefault(p => p.Status == update.Status);
                    if (preference == null)
                    {
                        Console.WriteLine($"❌ Column {update.Status} not found in database");
                        return false;
                    }

                    preference.Order = update.Order;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"❌ Error: {ex.Message}");
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
    var defaultColumns = new List<UserColumnPreference>
    {
        new() { UserId = userId, Status = Status.Todo, DisplayName = "Todo", Order = 0 },
        new() { UserId = userId, Status = Status.InProgress, DisplayName = "In Progress", Order = 1 },
        new() { UserId = userId, Status = Status.Done, DisplayName = "Done", Order = 2 },
        new() { UserId = userId, Status = Status.Cancelled, DisplayName = "Cancelled", Order = 3 }
    };

    await _userColumnPreferenceRepository.AddRangeAsync(defaultColumns);
}
    }

}
