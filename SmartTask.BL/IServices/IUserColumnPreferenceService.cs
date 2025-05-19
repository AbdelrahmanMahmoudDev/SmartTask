using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTask.Core.Models.Enums;
using SmartTask.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.BL.Services
{
    public interface IUserColumnPreferenceService
    {
        Task<List<UserColumnPreference>> GetUserColumns(string userId);
        Task<bool> UpdateColumnOrder(string userId, List<ColumnOrderUpdate> columnOrder);
        Task<bool> UpdateDisplayName(string userId, Status status, string displayName);
        Task InitializeDefaultColumns(string userId);
    }

}
