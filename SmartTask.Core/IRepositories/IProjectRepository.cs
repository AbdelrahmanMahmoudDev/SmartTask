using SmartTask.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project = SmartTask.Core.Models.Project;

namespace SmartTask.Core.IRepositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Project>> GetAllAsyncWithoutInclude();
        Task<Project> GetByIdAsync(int id);
        Task<Project> GetWithDetailsAsync(int id);
        Task<IEnumerable<Project>> GetByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Project>> GetByCreatedByIdAsync(string createdById);
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(string userId);
        Task<Project> AddAsync(Project project);
        System.Threading.Tasks.Task UpdateAsync(Project project);
        System.Threading.Tasks.Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        IQueryable<Project> GetQueryable();

        // To Display list of projects for a specific user
        Task<List<Project>> GetUserProjectsAsync(string userId);

        //To Diplay Details of a specific project for a specific user
        Task<Project> GetProjectByIdAsync(int id, string userId);
        List<ApplicationUser> GetMembers(int id);
    }
}