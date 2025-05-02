using System.Collections.Generic;
using System.Threading.Tasks;
using Attachment = SmartTask.Core.Models.Attachment;

namespace SmartTask.Core.IRepositories
{
    public interface IAttachmentRepository
    {
        Task<IEnumerable<Attachment>> GetAllAsync();
        Task<Attachment> GetByIdAsync(int id);
        Task<IEnumerable<Attachment>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<Attachment>> GetByUploadedByIdAsync(int uploadedById);
        Task<Attachment> AddAsync(Attachment attachment);
        Task UpdateAsync(Attachment attachment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
