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

namespace SmartTask.BL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectService(IProjectRepository projectRepository, UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
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

            return await PaginatedList<Project>.CreateAsync(query, page, pageSize);
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
            await _projectRepository.UpdateAsync(project);
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
                query = query.Where(p => p.DepartmentId == departmentId);
            }

            if (branchId > 0)
            {
                query = query.Where(p => p.BranchId == branchId);
            }


            return await PaginatedList<Project>.CreateAsync(query, page, pageSize);
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
    }
}
