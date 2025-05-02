using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.IRepositories;
using SmartTask.DataAccess.Data;
using AISuggestion = SmartTask.Core.Models.AISuggestion;


namespace SmartTask.DataAccess.Repositories
{
    public class AISuggestionRepository : IAISuggestionRepository
    {
        private readonly SmartTaskContext _context;

        public AISuggestionRepository(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AISuggestion>> GetAllAsync()
        {
            return await _context.AISuggestions
                                 .Include(a => a.Task)    
                                 .ToListAsync();
        }

        public async Task<AISuggestion> GetByIdAsync(int id)
        {
            return await _context.AISuggestions
                                 .Include(a => a.Task)
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AISuggestion>> GetByTaskIdAsync(int taskId)
        {
            return await _context.AISuggestions
                                 .Where(a => a.TaskId == taskId)
                                 .OrderByDescending(a => a.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<AISuggestion>> GetBySuggestionTypeAsync(string suggestionType)
        {
            return await _context.AISuggestions
                                 .Where(a => a.SuggestionType == suggestionType)
                                 .OrderByDescending(a => a.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<AISuggestion> AddAsync(AISuggestion aiSuggestion)
        {
            _context.AISuggestions.Add(aiSuggestion);
            await _context.SaveChangesAsync();
            return aiSuggestion;
        }

        public async Task UpdateAsync(AISuggestion aiSuggestion)
        {
            _context.Entry(aiSuggestion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var suggestion = await _context.AISuggestions.FindAsync(id);
            if (suggestion != null)
            {
                _context.AISuggestions.Remove(suggestion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AISuggestions.AnyAsync(a => a.Id == id);
        }
    }
}
