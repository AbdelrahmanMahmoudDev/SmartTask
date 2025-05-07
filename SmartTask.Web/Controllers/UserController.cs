using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
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

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _userService.UpdateAsync(user);
                if (!success) return NotFound();

                return RedirectToAction(nameof(Index));
            }

            return View(user);
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
