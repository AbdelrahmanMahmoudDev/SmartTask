using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SmartTask.Core.Models
{
    public class UserDashboardPreference
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public bool ShowRecentProjects { get; set; } = true;
        public bool ShowProjectStatus { get; set; } = true;
        public bool ShowUpcomingTasks { get; set; } = true;
        public bool ShowActivityChart { get; set; } = true;
        public int RecentProjectsCount { get; set; } = 5;

        [StringLength(20)]
        public string PreferredView { get; set; } = "grid"; 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginDate { get; set; }

        // Navigation property
        [BindNever]
        [ValidateNever]
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }


    }
}
