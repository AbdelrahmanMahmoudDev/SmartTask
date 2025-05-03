using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.Core.Models.BasePermission
{
    public class ApplicationRole : IdentityRole
    {
        public string Access { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
