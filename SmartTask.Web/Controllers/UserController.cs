using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTask.Bl.Services;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels;
using SmartTask.Web.ViewModels.BranchVM;
using System.Linq.Expressions;

namespace SmartTask.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        public UserController(IUserService userService, IDepartmentService departmentService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _userService = userService;
            _departmentService = departmentService;
        }
        public async Task<IActionResult> GetAll(string? searchString = null, int page = 1 , int pageSize = 10)
        {
            //var users = _userService.GetAll(page, pageSize); 
            //return View(users);

            Expression<Func<ApplicationUser, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                filter = u => u.FullName.Contains(searchString) || u.Email.Contains(searchString);
            }

            var users = await _userService.GetFilteredAsync(filter, page, pageSize);

            var viewModel = new UsersViewModel
            {
                Users = users,
                SearchString = searchString
            }; 

            return View(viewModel);
        }

        public async Task<IActionResult> WithoutDepartment(int page = 1, int pageSize = 10)
        {
            Expression<Func<ApplicationUser, bool>> filter = u => u.DepartmentId == null;

            var users = await _userService.GetFilteredAsync(filter, page, pageSize);

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = departments;

            var viewModel = new UsersViewModel
            {
                Users = users,
                SearchString = null
            };

            return View("WithoutDepartment", viewModel);
        }
        public async Task<IActionResult> Details(string id)
        {
            if(id == null)
            {
                return BadRequest("User ID cannot be null");
            }

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AssignToDepartment(string userId, int departmentId)
        {
            var success = await _userService.AssignUserToDepartmentAsync(userId, departmentId);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to assign user to department.");
                return RedirectToAction("WithoutDepartment");
            }
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();

            return RedirectToAction("WithoutDepartment");
        }

        [Authorize(Policy = "CanEditUser")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                DepartmentId = user.DepartmentId,
                    ExistingImagePath = user.ImagePath

            };

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", user.DepartmentId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanEditUser")]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var user = await _userService.GetByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.DepartmentId = model.DepartmentId;
            user.updatedAt = DateTime.UtcNow;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(model.ExistingImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", model.ExistingImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uploadsFolder = Path.Combine("wwwroot", "images", "users");
                Directory.CreateDirectory(uploadsFolder); 

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                user.ImagePath = $"/images/users/{uniqueFileName}";
            }

            var updateResult = await _userService.UpdateAsync(user);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Failed to update user data.");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required.");
                    return View(model);
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(model);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            return RedirectToAction(nameof(GetAll));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _userService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        

    }
}
