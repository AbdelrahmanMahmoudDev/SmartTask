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
    public class AuditRepository : IAuditRepository
    {
        private readonly SmartTaskContext _smartTaskContext;

        public AuditRepository(SmartTaskContext smartTaskContext)
        {
            _smartTaskContext = smartTaskContext;
        }
        public List<Audit> GetAllAudits()
        {
           
            return _smartTaskContext.Audits.ToList();
        }

        public List<Audit> GetAuditsByUserId(string userid)
        {
           return _smartTaskContext.Audits.Where(a => a.UserId == userid).ToList();
        }
    }
}
