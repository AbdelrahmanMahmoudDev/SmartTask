using SmartTask.Core.Models.BasePermission;
using System.Collections.Generic;

namespace SmartTask.BL.IServices
{
    public interface IMvcControllerDiscovery
    {
        IEnumerable<MvcControllerInfo> GetControllers();
    }
}