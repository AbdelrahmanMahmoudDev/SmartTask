using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTask.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.ViewModels
{
    public class AddEventAsTaskViewModel
    {
        public int EventId { get; set; }

        [Required]
        public int? ProjectId { get; set; }
        public Priority Priority { get; set; }

        public List<SelectListItem> Projects { get; set; } = new();

        public List<string> UserIds { get; set; } = new();

        [Required]
        [Remote("ValidateStartDate", "OutLook", AdditionalFields = "ProjectId",
        ErrorMessage = "Start date must be within the project timeline.")]
        public DateTime Start { get; set; }

        [Required]
        [Remote("ValidateEndDate", "OutLook", AdditionalFields = "ProjectId,Start",
        ErrorMessage = "End date must be within the project timeline and after the start date.")]
        public DateTime End { get; set; }
    }
}
