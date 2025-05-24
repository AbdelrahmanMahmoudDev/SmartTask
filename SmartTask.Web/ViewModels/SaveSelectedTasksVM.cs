using SmartTask.Core.Models;

namespace SmartTask.Web.ViewModels
{
    public class SaveSelectedTasksVM
    {
        public int SelectedTaskId { get; set; }
        public List<int> SelectedTaskIds { get; set; } = new();
        public List<DependencyType> DependencyTypes { get; set; }=Enum.GetValues(typeof(DependencyType)).Cast<DependencyType>().ToList();
    }
}