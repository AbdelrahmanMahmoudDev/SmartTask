using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Core.Models.BasePermission
{
    public class MvcControllerInfo
    {
        public string Id => $"{AreaName}:{Name}";

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string AreaName { get; set; }

        public IEnumerable<MvcActionInfo> Actions { get; set; }
    }
}
