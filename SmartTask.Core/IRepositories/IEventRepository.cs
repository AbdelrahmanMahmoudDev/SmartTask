using System.Collections.Generic;
using System.Threading.Tasks;
using Event = SmartTask.Core.Models.Event;

namespace SmartTask.Core.IRepositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event> GetByIdAsync(int id);
        Task<IEnumerable<Event>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<Event>> GetByImportedByIdAsync(string importedById);
        Task<Event> AddAsync(Event eventEntity);
        Task UpdateAsync(Event eventEntity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
