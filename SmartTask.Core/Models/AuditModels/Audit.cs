using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Core.Models.AuditModels
{
    public class Audit
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
    }
}
