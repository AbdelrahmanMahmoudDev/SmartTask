using SmartTask.Core.Models;
using Task = SmartTask.Core.Models.Task;

namespace SmartTask.Web.ViewModels.ProjectVM
{
    public class ProjectProgressDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProgressPercentage { get; set; }
        public int DaysLeft { get; set; }
        public List<ApplicationUser> Members { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int TodoTasks { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();

        public ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }
        public List<ProjectMember> ProjectMembers { get; set; }


        // get status display properties
        public (string TextColor, string BgColor, string Icon) GetStatusStyle(string status)
        {
            return status switch
            {
                "Completed" => ("text-success", "#e6f7ee", "fa-check-circle"),
                "In Progress" => ("text-primary", "#e3f2fd", "fa-spinner"),
                "Todo" => ("text-info", "#e6f9ff", "fa-list-alt"),
                "Not Started" => ("text-secondary", "#f0f0f0", "fa-clock"),
                _ => ("text-dark", "#f8f9fa", "fa-question-circle")
            };
        }

        // count tasks by status
        public int CountTasksByStatus(string status)
        {
            return Tasks.Count(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            //بتاخد التاسك وتشوف هل ال status بتاعه يساوي ال status اللي انت عايز تشوفه ولا لا
        }
    }
}
