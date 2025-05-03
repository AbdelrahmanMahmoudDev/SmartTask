using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTask.Core.Models.AuditModels;

namespace SmartTask.Core.IRepositories
{
    public interface IUserLoginHistoryRepository
    {
        public List<UserLoginHistory> GetAllUserLoginHistories();

    }
}
