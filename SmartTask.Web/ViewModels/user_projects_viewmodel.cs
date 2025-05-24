using SmartTask.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace SmartTask.Web.ViewModels
{
    /// <summary>
    /// View model representing user's project dashboard data
    /// </summary>
    public class UserProjectsViewModel
    {
        public List<ProjectSummaryViewModel> Projects { get; set; } = new List<ProjectSummaryViewModel>();
    }

    /// <summary>
    /// View model representing a project summary for the user
    /// </summary>
    public class ProjectSummaryViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        
        // Task statistics for the current user
        public int TotalTasksAssignedToUser { get; set; }
        public int CompletedTasksByUser { get; set; }
        public int InProgressTasksByUser { get; set; }
        
        // Completion percentage calculation
        public double CompletionPercentage 
        { 
            get 
            { 
                if (TotalTasksAssignedToUser == 0) return 0;
                return Math.Round((double)CompletedTasksByUser / TotalTasksAssignedToUser * 100, 2);
            } 
        }

        // User's current in-progress tasks
        public List<InProgressTaskViewModel> InProgressTasks { get; set; } = new List<InProgressTaskViewModel>();
        
        // User's upcoming tasks (not started yet)
        public List<UpcomingTaskViewModel> UpcomingTasks { get; set; } = new List<UpcomingTaskViewModel>();
    }

    /// <summary>
    /// View model representing an in-progress task
    /// </summary>
    public class InProgressTaskViewModel
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
        public Priority Priority { get; set; }
        
        // Indicates if task end date has passed the project end date
        public bool IsDelayed { get; set; }
    }

    /// <summary>
    /// View model representing an upcoming task that hasn't started yet
    /// </summary>
    public class UpcomingTaskViewModel
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}