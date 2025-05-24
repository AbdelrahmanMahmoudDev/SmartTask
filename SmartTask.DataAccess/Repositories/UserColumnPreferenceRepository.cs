using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.Enums;
using SmartTask.DataAccess.Data;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.DataAccess.Repositories
{
    public class UserColumnPreferenceRepository : IUserColumnPreferenceRepository
    {
        private readonly SmartTaskContext _context;

        public UserColumnPreferenceRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<List<UserColumnPreference>> GetByUserId(string userId)
        {
            return await _context.UserColumnPreferences
                .Where(u => u.UserId == userId)
                .OrderBy(u => u.Order)
                .ToListAsync();
        }

        public async Task<UserColumnPreference> GetByUserIdAndStatus(string userId, Status status)
        {
            return await _context.UserColumnPreferences
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Status == status);
        }

        public async Task UpdateAsync(UserColumnPreference preference)
        {
            _context.Entry(preference).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<UserColumnPreference> preferences)
        {
            await _context.UserColumnPreferences.AddRangeAsync(preferences);
            await _context.SaveChangesAsync();
        }


        public void Update(UserColumnPreference preference)
        {
            _context.Entry(preference).State = EntityState.Modified;
        }
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

    }
}
