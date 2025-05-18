using Microsoft.AspNetCore.Authorization;

namespace SmartTask.Web.Authorization.Requirements
{
    // Requirement for adding members to projects
    public class ManageProjectMembersRequirement : IAuthorizationRequirement
    {
    }
}
