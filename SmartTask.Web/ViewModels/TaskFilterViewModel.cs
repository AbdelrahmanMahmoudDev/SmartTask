using System.ComponentModel.DataAnnotations;
using SmartTask.Core.Models.Enums;

namespace SmartTask.Web.ViewModels
{
    public class TaskFilterViewModel
    {
        public Status Status { get; set; }
        public string AssignedToUserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public int? DepartmentId { get; set; }
        public int? BranchId { get; set; }

        public List<Task> FilteredTasks { get; set; }
    }
}
