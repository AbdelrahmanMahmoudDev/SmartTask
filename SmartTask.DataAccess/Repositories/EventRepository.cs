using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.DataAccess.Data;
using SmartTask.Core.IRepositories;
using Event = SmartTask.Core.Models.Event;


namespace SmartTask.DataAccess.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly SmartTaskContext _context;

        public EventRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Task)
                .Include(e => e.ImportedBy)
                .ToListAsync();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Task)
                .Include(e => e.ImportedBy)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetByTaskIdAsync(int taskId)
        {
            return await _context.Events
                .Include(e => e.ImportedBy)
                .Where(e => e.TaskId == taskId)
                .OrderBy(e => e.Start)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetByImportedByIdAsync(int importedById)
        {
            return await _context.Events
                .Include(e => e.Task)
                .Where(e => e.ImportedById == importedById)
                .OrderBy(e => e.Start)
                .ToListAsync();
        }

        public async Task<Event> AddAsync(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task UpdateAsync(Event eventEntity)
        {
            _context.Entry(eventEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity != null)
            {
                _context.Events.Remove(eventEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }
    }
}