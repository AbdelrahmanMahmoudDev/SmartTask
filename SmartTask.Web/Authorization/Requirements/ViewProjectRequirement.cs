using Microsoft.AspNetCore.Authorization;

namespace SmartTask.Web.Authorization.Requirements
{
    // Requirement for viewing project details
    public class ViewProjectRequirement : IAuthorizationRequirement
    {
    }
}