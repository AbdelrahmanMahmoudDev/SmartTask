using SmartTask.Core.Models.Enums;

namespace SmartTask.Web.ViewModels
{
    public class CalendarViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }
        public Status TaskStatus { get; set; }
    }
}
