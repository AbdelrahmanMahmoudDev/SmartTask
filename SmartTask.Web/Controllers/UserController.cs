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
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        public UserController(IUserService userService, IDepartmentService departmentService)
        {
            _userService = userService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchString = null, int page = 1 , int pageSize = 10)
        {
            var users = await _userService.GetFilteredAsync(searchString, page, pageSize);

            var viewModel = new UsersViewModel
            {
                Users = users,
                SearchString = searchString
            }; 

            return View(viewModel);
        }

        public async Task<IActionResult> WithoutDepartment(int page = 1, int pageSize = 10)
        {

            var users = await _userService.GetUsersWithoutDepartemnt(page,pageSize);

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
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
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

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
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
