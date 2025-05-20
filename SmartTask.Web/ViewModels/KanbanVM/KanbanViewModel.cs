using SmartTask.Core.Models;
using TaskModel = SmartTask.Core.Models.Task;

namespace SmartTask.Web.ViewModels.KanbanVM
{
    public class KanbanViewModel
    {
        public IEnumerable<TaskModel> Tasks { get; set; }
        public IEnumerable<UserColumnPreference> Columns { get; set; }
    }
}
