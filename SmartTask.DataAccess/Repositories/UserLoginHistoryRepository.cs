using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models.AuditModels;
using SmartTask.DataAccess.Data;

namespace SmartTask.DataAccess.Repositories
{
    public class UserLoginHistoryRepository:IUserLoginHistoryRepository
    {
        private readonly SmartTaskContext _smartTaskContext;

        public UserLoginHistoryRepository(SmartTaskContext smartTaskContext)
        {
            _smartTaskContext = smartTaskContext;
        }

        public List<UserLoginHistory> GetAllUserLoginHistories()
        {
           return _smartTaskContext.UserLoginHistories.ToList();
        }
        public void AddUserLoginHistory(UserLoginHistory userLoginHistory)
        {
            _smartTaskContext.UserLoginHistories.Add(userLoginHistory);
            _smartTaskContext.SaveChanges();
        }

    }
}
