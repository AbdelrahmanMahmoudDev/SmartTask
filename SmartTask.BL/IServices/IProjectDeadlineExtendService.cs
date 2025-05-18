using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.IServices
{
    public interface IProjectDeadlineExtendService
    {
        Task ExtendProjectDeadlineToFitTasks(int project_id);
    }
}
