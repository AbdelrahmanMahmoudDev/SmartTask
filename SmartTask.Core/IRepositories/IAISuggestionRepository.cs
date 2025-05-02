using System.Collections.Generic;
using System.Threading.Tasks;
using AISuggestion = SmartTask.Core.Models.AISuggestion;

namespace SmartTask.Core.IRepositories
{
    public interface IAISuggestionRepository
    {
        Task<IEnumerable<AISuggestion>> GetAllAsync();
        Task<AISuggestion> GetByIdAsync(int id);
        Task<IEnumerable<AISuggestion>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<AISuggestion>> GetBySuggestionTypeAsync(string suggestionType);
        Task<AISuggestion> AddAsync(AISuggestion aiSuggestion);
        Task UpdateAsync(AISuggestion aiSuggestion);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
