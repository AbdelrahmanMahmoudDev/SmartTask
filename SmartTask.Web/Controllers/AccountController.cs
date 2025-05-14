using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.AuditModels;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Core.ViewModels;
using System.Data.SqlTypes;
using SmartTask.BL.Services;
namespace SmartTask.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IUserLoginHistoryRepository _userLoginHistory;
        IConfiguration _config;
        public AccountController(UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager, RoleManager<ApplicationRole> _roleManager,
            IUserLoginHistoryRepository userLoginHistory, IConfiguration config)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            _userLoginHistory = userLoginHistory;
            _config = config;
        }
        [Authorize]
        public IActionResult Outlook()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("OutlookCallback")
            };

            // Add `prompt=login` to force the user to sign in again
            props.Items["LoginHint"] = ""; // optional
            props.Items["prompt"] = "login";

            return Challenge(props, "Outlook");
        }
        public async Task<IActionResult> OutlookCallback()
        {
            var accessToken = await HttpContext.GetTokenAsync("Outlook", "access_token");
            var refreshToken = await HttpContext.GetTokenAsync("Outlook", "refresh_token");
            var expiresAt = await HttpContext.GetTokenAsync("Outlook", "expires_at");

            HttpContext.Session.SetString("access_token", accessToken);
            HttpContext.Session.SetString("refresh_token", refreshToken);
            HttpContext.Session.SetString("expires_at", expiresAt);

            return RedirectToAction("cal");
        }


        [Authorize]
        public async Task<IActionResult> cal()
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            var refreshToken = HttpContext.Session.GetString("refresh_token");
            var expiresAtStr = HttpContext.Session.GetString("expires_at");

            if (DateTime.TryParse(expiresAtStr, out var expiresAt))
            {
                if (DateTime.UtcNow > expiresAt)
                {
                    accessToken = await RefreshAccessTokenAsync(refreshToken);
                    HttpContext.Session.SetString("access_token", accessToken);
                    HttpContext.Session.SetString("expires_at", DateTime.UtcNow.AddMinutes(60).ToString());
                }
            }

            // Ensure the required NuGet packages are installed:  
            // 1. Microsoft.Graph  
            // 2. Microsoft.Graph.Auth  
            // 3. Microsoft.Identity.Client  

            var authProvider = new MyAccessTokenProvider(accessToken);

            // Create the GraphServiceClient using your custom provider
            var graphClient = new GraphServiceClient(authProvider);

            var events = await graphClient.Me.Events.GetAsync(opt =>
            {
                
                //opt.QueryParameters.Top = 10;
                //opt.QueryParameters.Select = new[] { "subject", "start", "end" };
                opt.QueryParameters.Orderby = new[] { "start/dateTime" };
            });

            return View(events.Value);
        }

        private async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            var http = new HttpClient();
            var parameters = new Dictionary<string, string>
        {
            { "client_id", _config["AzureAd:ClientId"] },
            { "client_secret", _config["AzureAd:ClientSecret"] },
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "scope", "https://graph.microsoft.com/.default offline_access" }
        };

            var res = await http.PostAsync($"https://login.microsoftonline.com/{_config["AzureAd:TenantId"]}/oauth2/v2.0/token", new FormUrlEncodedContent(parameters));
            var tokenResult = await res.Content.ReadFromJsonAsync<OAuthResponse>();

            return tokenResult.access_token;
        }




        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel account)
        {
            ApplicationUser user = new ApplicationUser();   
            //Console.WriteLine($"{account.UserName}");
            if (ModelState.IsValid)
            {
                try
                {
                     user = await userManager.FindByNameAsync(account.UserName);
                    // ApplicationUser user = await userManager.FindByNameAsync("mohamedali");
                }
                catch (SqlNullValueException ex)
                {
                    Console.WriteLine("Caught null field: " + ex.Message);
                }
                if (user != null)
                {
                    bool result = await userManager.CheckPasswordAsync(user, account.Password);
                    if (result)
                    {
                        await signInManager.SignInAsync(user, isPersistent: account.RememberMe);
                        _userLoginHistory.AddUserLoginHistory(new UserLoginHistory
                        {
                            UserId = user.Id,
                            LoginTime = DateTime.Now,
                            IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                            UserName = user.UserName
                        });
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid Email or Password");
            }
            return View("Login", account);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser();
                applicationUser.UserName = register.UserName;
                applicationUser.FullName = register.FName;
                applicationUser.Email = register.Email;
                applicationUser.PhoneNumber = register.PhoneNumber;
                applicationUser.createdAt = DateTime.Now;
                applicationUser.updatedAt = DateTime.Now;

                IdentityResult identityResult = await userManager.CreateAsync(applicationUser, register.Password);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View("Register", register);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
/*
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ViewBag.Message = "Role name is required";
            }
            else
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    ViewBag.Message = "Role already exists";
                }
                else
                {
                    var newRole = new ApplicationRole
                    {
                        Name = roleName.Trim(),
                        NormalizedName = roleName.Trim().ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };

                    var result = await roleManager.CreateAsync(newRole);

                    ViewBag.Message = result.Succeeded ? "Role created successfully!" : "Failed to create role!";
                }
            }

            return View();
        }
        */
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            var users = await userManager.Users.ToListAsync();
            var roles = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            var model = new List<UserRoleAssignmentViewModel>();

            foreach (var user in users)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                model.Add(new UserRoleAssignmentViewModel
                {
                    UserId = user.Id,
                    UserName = user.FullName,
                    Roles = roles.Select(role => new RoleCheckbox
                    {
                        RoleName = role,
                        IsAssigned = userRoles.Contains(role)
                    }).ToList()
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRoleAssignmentViewModel> model)
        {
            foreach (var userModel in model)
            {
                var user = await userManager.FindByIdAsync(userModel.UserId);
                if (user == null) continue;

                var currentRoles = await userManager.GetRolesAsync(user);

                foreach (var role in userModel.Roles)
                {
                    if (role.IsAssigned && !currentRoles.Contains(role.RoleName))
                        await userManager.AddToRoleAsync(user, role.RoleName);

                    if (!role.IsAssigned && currentRoles.Contains(role.RoleName))
                        await userManager.RemoveFromRoleAsync(user, role.RoleName);
                }
            }

            TempData["Message"] = "Roles updated successfully!";
            return RedirectToAction("ManageUserRoles");
        }
    }
    public class OAuthResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}