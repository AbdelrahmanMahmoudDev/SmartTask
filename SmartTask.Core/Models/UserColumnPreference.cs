using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTask.Core.Models.Enums;

namespace SmartTask.Core.Models
{
    public class UserColumnPreference
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Status Status { get; set; }
        public string DisplayName { get; set; }
        public int Order { get; set; }
    }
}
