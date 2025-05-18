using System.Collections.Generic;
using System.Threading.Tasks;
using Project = SmartTask.Core.Models.Project;

namespace SmartTask.Core.IRepositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project> GetByIdAsync(int id);
        Task<Project> GetWithDetailsAsync(int id);
        Task<IEnumerable<Project>> GetByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Project>> GetByCreatedByIdAsync(string createdById);
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(string userId);
        Task<Project> AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        IQueryable<Project> GetQueryable();

        // To Display list of projects for a specific user
        Task<List<Project>> GetUserProjectsAsync(string userId);

        //To Diplay Details of a specific project for a specific user
        Task<Project> GetProjectByIdAsync(int id, string userId);
    }
}