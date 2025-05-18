using SmartTask.Core.IRepositories;
using SmartTask.Web.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SmartTask.Web.CustomeValidations
{
    public class CustomDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (AddEventAsTaskViewModel)validationContext.ObjectInstance;

            if (model.ProjectId == null)
                return new ValidationResult("Please select a project.");

            // Get service from DI
            var serviceProvider = validationContext.GetService<IServiceProvider>();
            var projectService = serviceProvider.GetRequiredService<IProjectRepository>();
            var project = projectService.GetByIdAsync(model.ProjectId.Value).Result;

            if (project == null)
                return new ValidationResult("Invalid project selected.");

            if (model.Start < project.StartDate || model.End > project.EndDate)
            {
                return new ValidationResult($"Start and End dates must be between {project.StartDate:yyyy-MM-dd} and {project.EndDate:yyyy-MM-dd}.");
            }

            if (model.End < model.Start)
            {
                return new ValidationResult("End date must be after Start date.");
            }

            return ValidationResult.Success!;
        }
    }
}
