using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Core.Models.AuditModels
{
    public class UserLoginHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public DateTime LoginTime { get; set; }

        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
