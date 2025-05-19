using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = System.Threading.Tasks.Task;
using SmartTask.Core.Models;
using SmartTask.Bl.Helpers;

namespace SmartTask.BL.IServices
{
    public interface IProjectStatusService
    {
        Task ArchiveProjectTasksAsync(int projectId);
        Task RestoreProjectTasksAsync(int projectId);
        Task HandleProjectStatusChangeAsync(Project project, string oldStatus);
    }
}