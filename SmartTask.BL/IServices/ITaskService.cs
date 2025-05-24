using Microsoft.AspNetCore.Http;
using SmartTask.Core.Models;
using SmartTask.Core.Models.ServiceDto;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.BL.IServices
{
    public interface ITaskService
    {
        Task<Core.Models.Task> Details(int id);
        Task<bool> ISAParent(int id);

        Task<Comment> AddComment(int taskId, string authorId, string content);

        Task<Attachment> AddAttachment(int taskId, IFormFile file, string userId, string webRootPath);

        Task<List<Core.Models.Task>> TasksForUserInProject(int projectId, string userId);

        Task<List<Core.Models.Task>> TasksForProject(int projectId);

        Task<List<Core.Models.Task>> TasksForUser(string userId);

        Task<int> NumofDepend(int taskId);

        Task<Core.Models.Task> GetTask(int taskId);

        Task DeleteDepend(int taskId);

        Task Delete(int taskId);

        Task<List<TaskDenpendDto>> Loadnodes(int id);

        Task SaveSelectedTasks(int selectedTaskId, List<int> selectedTaskIds,List<DependencyType> dependencyTypes) ;
    }
}