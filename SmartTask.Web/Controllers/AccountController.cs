using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Core.ViewModels;
using System.Data.SqlTypes;

namespace SmartTask.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, RoleManager<ApplicationRole> _roleManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
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
            Console.WriteLine($"{account.UserName}");
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
}