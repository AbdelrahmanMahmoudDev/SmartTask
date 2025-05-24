using SmartTask.Core.Models;

namespace SmartTask.Api.DTOs
{
    public class SaveSelectedTasksDTO
    {
        public int SelectedTaskId { get; set; }
        public List<int> SelectedTaskIds { get; set; } = new();
        public List<DependencyType> DependencyType { get; set; } = new();

    }
}