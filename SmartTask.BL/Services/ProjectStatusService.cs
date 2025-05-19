using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = System.Threading.Tasks.Task;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Bl.Helpers;
using Microsoft.AspNetCore.Identity;
using TaskModel = SmartTask.Core.Models.Task;


namespace SmartTask.BL.Services
{
    public class ProjectStatusService : IProjectStatusService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly Dictionary<int, string> _originalTaskStatuses;

        public ProjectStatusService(ITaskRepository taskRepository, IProjectRepository projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _originalTaskStatuses = new Dictionary<int, Status>();
        }

        public async Task ArchiveProjectTasksAsync(int projectId)
        {
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

            foreach (var task in tasks)
            {
                // Preserve the original status of the task before archiving
                _originalTaskStatuses[task.Id] = task.Status;

                // Change the task status to "Archived"
                task.Status = Status.Archived;
                await _taskRepository.UpdateAsync(task);
            }

            // Original statuses can be stored in the database or external store if persistence across application restarts is required
        }

        public async Task RestoreProjectTasksAsync(int projectId)
        {
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

            foreach (var task in tasks)
            {
                // Restore the status if an original value is available
                if (_originalTaskStatuses.ContainsKey(task.Id))
                {
                    task.Status = _originalTaskStatuses[task.Id];
                    _originalTaskStatuses.Remove(task.Id);
                }
                else
                {
                    // If no original status is found, revert to the default status (e.g., Pending)
                    task.Status = Status.Pending;
                }

                await _taskRepository.UpdateAsync(task);
            }
        }

        public async Task HandleProjectStatusChangeAsync(Project project, string oldStatus)
        {
            // When the project status is changed to "Archived"
            if (project.Status == "Archived" && oldStatus != "Archived")
            {
                await ArchiveProjectTasksAsync(project.Id);
            }
            // When the project status is changed from "Archived" to any other state
            else if (oldStatus == "Archived" && project.Status != "Archived")
            {
                await RestoreProjectTasksAsync(project.Id);
            }
        }
    }
}
