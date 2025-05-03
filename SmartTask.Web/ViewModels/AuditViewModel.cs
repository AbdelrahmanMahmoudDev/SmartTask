using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels
{
    public class AuditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "User ID")]
        public string UserId { get; set; }
        [Display(Name = "Table Name")]
        public string TableName { get; set; }

        public string Action { get; set; }
        [Display(Name = "User Name")]
        public string Username { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime Timestamp { get; set; }
        [Display(Name = "Old Values")]
        public string OldValues { get; set; }
        [Display(Name = "New Values")]
        public string NewValues { get; set; }
        [Display(Name = "Affected Columns")]
        public string AffectedColumns { get; set; }

    }
}
