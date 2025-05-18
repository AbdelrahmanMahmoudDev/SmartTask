using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Web.ViewModels;

namespace SmartTask.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IDashboardService _dashboardService;

        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, RoleManager<ApplicationRole> _roleManager, IDashboardService _dashboardService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            this._dashboardService = _dashboardService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel account)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByNameAsync(account.UserName);
                if (user != null)
                {
                    bool result = await userManager.CheckPasswordAsync(user, account.Password);
                    if (result)
                    {
                        await signInManager.SignInAsync(user, isPersistent: account.RememberMe);
                        //DashboardService
                        var preference = await _dashboardService.GetUserDashboardSettingsAsync(user.Id);
                        preference.LastLoginDate = DateTime.Now;
                        await _dashboardService.UpdateUserPreferenceAsync(preference);

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

                if (register.UserImage != null && register.UserImage.Length > 0)
                {
                    // Check if file size exceeds 1MB (1,048,576 bytes)
                    if (register.UserImage.Length > 1048576)
                    {
                        ModelState.AddModelError("UserImage", "Image size cannot exceed 1MB.");
                        return View("Register", register);
                    }

                    try
                    {
                        // Process the image file
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(register.UserImage.FileName);
                        var imagesFolder = Path.Combine("wwwroot", "assets", "img", "user-img");
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), imagesFolder, uniqueFileName);

                        // Ensure the directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                        // Save the file
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await register.UserImage.CopyToAsync(stream);
                        }

                        // Set the image path to be saved in the database
                        string savedPath = "/assets/img/user-img/" + uniqueFileName;
                        register.ImagePath = savedPath;
                        applicationUser.ImagePath = savedPath;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error saving image: {ex.Message}");
                        return View("Register", register);
                    }
                }


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

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ApplicationUser user, [FromForm] string password)
        {
            // Validate file size
            if (user.UserImage != null && user.UserImage.Length > 0)
            {
                // Check if file size exceeds 1MB
                if (user.UserImage.Length > 1048576)
                {
                    return BadRequest(new { error = "Image size cannot exceed 1MB." });
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(user.UserImage.FileName);
                var imagesFolder = Path.Combine("wwwroot", "assets", "img", "user-img");
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), imagesFolder, uniqueFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await user.UserImage.CopyToAsync(stream);
                }
                user.ImagePath = "/assets/img/user-img" + uniqueFileName;
            }

            // Set creation and update timestamps
            user.createdAt = DateTime.Now;
            user.updatedAt = DateTime.Now;

            // Since we're using UserManager for user creation, we can use it directly
            IdentityResult result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Return success response, avoid referencing GetByIdAsync if it doesn't exist
                return CreatedAtAction("Get", "Users", new { id = user.Id }, user);
            }

            return BadRequest(result.Errors);
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

        // This action handles the AccessDenied scenario
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

    }
}