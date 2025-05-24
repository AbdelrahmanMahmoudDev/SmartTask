using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SmartTask.Core.Models;
using SmartTask.Core.Models.Enums;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.Core.IRepositories
{
    public interface IUserColumnPreferenceRepository
    {
        Task<List<UserColumnPreference>> GetByUserId(string userId);
        Task<UserColumnPreference> GetByUserIdAndStatus(string userId, Status status);
        Task UpdateAsync(UserColumnPreference preference);
        Task AddRangeAsync(IEnumerable<UserColumnPreference> preferences);

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveChangesAsync();
        void Update(UserColumnPreference preference);
    }

}
