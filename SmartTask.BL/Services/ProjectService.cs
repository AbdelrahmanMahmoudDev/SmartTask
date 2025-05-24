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
using SmartTask.Core.Models.Enums;


namespace SmartTask.BL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectService(
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        public async Task< PaginatedList<Project>> GetFilteredProjectsAsync(string searchString, int page, int pageSize)
        public async Task<PaginatedList<Project>> GetFilteredProjectsAsync(string searchString, int page, int pageSize)
        {
            var query = _projectRepository.GetQueryable();
            

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description.Contains(searchString));
            }

            return  await PaginatedList<Project>.CreateAsync(query, page, pageSize);
        }


        public async Task<Project> AddProjectAsync(Project project)
        {
            return await _projectRepository.AddAsync(project);
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _projectRepository.GetAllAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            // Get the existing project from database to compare status
            var existingProject = await _projectRepository.GetWithDetailsAsync(project.Id);

            if (existingProject == null)
            {
                throw new InvalidOperationException($"Project with ID {project.Id} not found.");
            }

            bool isArchiving = existingProject.Status != "Archived" && project.Status == "Archived";
            bool isUnarchiving = existingProject.Status == "Archived" && project.Status != "Archived";

            // Update the project
            await _projectRepository.UpdateAsync(project);

            // Handle task statuses based on project status change
            if (isArchiving)
            {
                await ArchiveProjectTasksAsync(project.Id);
        }
            else if (isUnarchiving)
            {
                await UnarchiveProjectTasksAsync(project.Id);
            }
        }

        private async Task ArchiveProjectTasksAsync(int projectId)
        {
            // Get all tasks for the project
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

            foreach (var task in tasks)
            {
                // Store the previous status in the database before changing it
                // We'll use a custom property in the task description to store the previous status
                // Format: "PREV_STATUS:{status}|{originalDescription}"
                string originalDescription = task.Description ?? string.Empty;

                // Store the previous status code in the description field
                task.Description = $"PREV_STATUS:{(int)task.Status}|{originalDescription}";

                // Set status to Archived (Status code 5 as per enum)
                task.Status = Status.Archived;

                // Update the task in the database
                await _taskRepository.UpdateAsync(task);
            }
        }

        private async Task UnarchiveProjectTasksAsync(int projectId)
        {
            // Get all tasks for the project
            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

            foreach (var task in tasks)
            {
                // Only restore status if the task is currently archived
                if (task.Status == Status.Archived)
                {
                    string description = task.Description ?? string.Empty;

                    // Check if the description contains our status marker
                    if (description.StartsWith("PREV_STATUS:"))
                    {
                        try
                        {
                            // Extract the previous status and original description
                            int pipeIndex = description.IndexOf('|');
                            if (pipeIndex > 0)
                            {
                                string statusPart = description.Substring(12, pipeIndex - 12); // 12 is the length of "PREV_STATUS:"

                                if (int.TryParse(statusPart, out int previousStatusValue))
                                {
                                    // Restore the previous status if valid
                                    if (Enum.IsDefined(typeof(Status), previousStatusValue))
                                    {
                                        task.Status = (Status)previousStatusValue;
                                    }

                                    // Restore the original description
                                    task.Description = description.Substring(pipeIndex + 1);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // If there's any error parsing, leave the description as is
                            // and default the status to Todo
                            task.Status = Status.Todo;
                        }
                    }
                    else
                    {
                        // If no previous status was stored, default to Todo
                        task.Status = Status.Todo;
                    }

                    // Update the task in the database
                    await _taskRepository.UpdateAsync(task);
                }
            }
        }

        public async Task DeleteProjectAsync(int id)
        {
            await _projectRepository.DeleteAsync(id);
        }

        public async Task<PaginatedList<Project>> GetFilteredByDepartmentProjectsAsync(string searchString, int? departmentId, int? branchId, int page, int pageSize)
        {
            var query = _projectRepository.GetQueryable();


            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description.Contains(searchString));

            }

            if (departmentId > 0)
            {
                query = query.Where(p => p.ProjectMembers.Any(x => x.User.DepartmentId == departmentId));
            }

            if (branchId > 0)
            {
                query = query.Where(p => p.ProjectMembers.Any(x => x.User.BranchId == branchId));
            }


            return await  PaginatedList<Project>.CreateAsync(query, page, pageSize);

            return PaginatedList<Project>.Create(query, page, pageSize);
        }

        public async Task<bool> AddMemberAsync(int projectId, string userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null || await _userManager.FindByIdAsync(userId) == null)
                return false;

            var existingMember = project.ProjectMembers.FirstOrDefault(pm => pm.UserId == userId);
            if (existingMember != null) return true; 

            project.ProjectMembers.Add(new ProjectMember
            {
                ProjectId = projectId,
                UserId = userId
            });

            await _projectRepository.UpdateAsync(project);
            return true;
        }

        public Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            return _projectRepository.GetUserProjectsAsync(userId);
        }

        public Task<Project> GetProjectDetailsAsync(int projectId, string userId)
        {
            return _projectRepository.GetProjectByIdAsync(projectId, userId);
        }
    }
}
