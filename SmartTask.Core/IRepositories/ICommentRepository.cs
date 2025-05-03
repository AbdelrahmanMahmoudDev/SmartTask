using System.Collections.Generic;
using System.Threading.Tasks;
using Comment = SmartTask.Core.Models.Comment;

namespace SmartTask.Core.IRepositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<Comment> GetByIdAsync(int id);
        Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<Comment>> GetByAuthorIdAsync(string authorId);
        Task<Comment> AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
