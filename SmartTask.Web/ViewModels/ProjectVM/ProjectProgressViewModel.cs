using SmartTask.Core.Models;
using Task = SmartTask.Core.Models.Task;

namespace SmartTask.Web.ViewModels.ProjectVM
{
    public class ProjectProgressViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ProgressPercentage { get; set; }
        public int DaysLeft { get; set; }
        public int MembersCount { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int TodoTasks { get; set; }

        public List<Task> Tasks { get; set; } = new List<Task>();// For project tasks
        public List<ProjectMember> ProjectMembers { get; set; } // For project members 
        public string OwnerId { get; set; }
    }
}
