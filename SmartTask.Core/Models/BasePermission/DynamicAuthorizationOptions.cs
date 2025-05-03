using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Core.Models.BasePermission
{
    public class DynamicAuthorizationOptions
    {
        /// <summary>
        /// Sets the default admin user. Authorization check will be suppressed.
        /// </summary>
        /// <value>The default admin user.</value>
        public string DefaultAdminUser { get; set; }
    }
}
