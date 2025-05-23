﻿
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartTask.Core.Models.BasePermission;
using SmartTask.DataAccess.Data;

namespace SmartTask.Web.TagHelpers
{
    [HtmlTargetElement("secure-content")]
    public class SecureContentTagHelper : TagHelper
    {
        private readonly SmartTaskContext _dbContext;
        private readonly DynamicAuthorizationOptions _authorizationOptions;

        public SecureContentTagHelper(SmartTaskContext dbContext, DynamicAuthorizationOptions authorizationOptions)
        {
            _dbContext = dbContext;
            _authorizationOptions = authorizationOptions;
        }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            var user = ViewContext.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                output.SuppressOutput();
                return;
            }

            if (user.Identity.Name.Equals(_authorizationOptions.DefaultAdminUser, StringComparison.CurrentCultureIgnoreCase))
                return;

            var roles = await (
                from usr in _dbContext.Users
                join userRole in _dbContext.UserRoles on usr.Id equals userRole.UserId
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                where usr.UserName == user.Identity.Name
                select role
            ).ToListAsync();

            var actionId = $"{Area}:{Controller}:{Action}";

            foreach (var role in roles)
            {
                if (role.Access == null)
                    continue;

                var accessList = JsonConvert.DeserializeObject<IEnumerable<MvcControllerInfo>>(role.Access);
                if (accessList.SelectMany(c => c.Actions).Any(a => a.Id == actionId))
                    return;
            }

            output.SuppressOutput();
        }
    }
}