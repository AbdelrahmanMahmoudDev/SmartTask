using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = System.Threading.Tasks.Task;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Bl.Helpers;

namespace SmartTask.BL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task< PaginatedList<Project>> GetFilteredProjectsAsync(string searchString, int page, int pageSize)
        {
            var query = _projectRepository.GetQueryable();
            

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description.Contains(searchString));
            }

            return PaginatedList<Project>.Create(query, page, pageSize);
        }


        public async Task<Project> AddProjectAsync(Project project)
        {
            return await _projectRepository.AddAsync(project);
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await _projectRepository.GetAllAsync();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            await _projectRepository.UpdateAsync(project);
        }

        public async Task DeleteProjectAsync(int id)
        {
            await _projectRepository.DeleteAsync(id);
        }
    }
}
