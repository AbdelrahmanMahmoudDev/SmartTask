using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels.DepartmentVM;
using Task = System.Threading.Tasks.Task;

namespace SmartTask.Web.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentController(
            IDepartmentService departmentService,
            UserManager<ApplicationUser> userManager)
        {
            _departmentService = departmentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 4)
        {
            var departments = await _departmentService.GetFilteredDepartments(searchString, page, pageSize);

            var viewModel = new DepartmentIndexViewModel
            {
                Departments = departments,
                SearchString = searchString
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateViewBagsForDepartment();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagsForDepartment(model.ManagerId);
                return View(model);
            }

            var department = new Department
            {
                Name = model.Name,
                ManagerId = model.ManagerId
            };

            if (model.SelectedUserIds != null && model.SelectedUserIds.Any())
            {
                //foreach (var userId in model.SelectedUserIds)
                //{
                //    var user = await _userManager.FindByIdAsync(userId);
                //    if (user != null)
                //    {
                //        user.DepartmentId = department.Id;
                //        await _userManager.UpdateAsync(user);
                //    }
                //}

                department.Users = await _userManager.Users
                    .Where(u => model.SelectedUserIds.Contains(u.Id))
                    .ToListAsync();
            }

            await _departmentService.AddDepartmentAsync(department);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentService.GetDepartmentWithDetailsAsync(id);
            if (department == null)
            {
                return View("NotFound");
            }

            var model = new DepartmentDetailsViewModel
            {
                Id = department.Id,
                Name = department.Name,
                ManagerName = department.Manager?.FullName ?? "No Manager Assigned",
                Users = department.Users.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return View("NotFound");
            }

            await PopulateViewBagsForDepartment(department.ManagerId);

            var model = new DepartmentFormViewModel
            {
                Id = department.Id,
                Name = department.Name,
                ManagerId = department.ManagerId,
                SelectedUserIds = department.Users.Select(u => u.Id).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagsForDepartment(model.ManagerId);
                return View(model);
            }

            var existingDepartment = await _departmentService.GetDepartmentByIdAsync(model.Id);
            if (existingDepartment == null)
            {
                return View("NotFound");
            }

            existingDepartment.Name = model.Name;
            existingDepartment.ManagerId = model.ManagerId;

            var selectedUserIds = model.SelectedUserIds ?? new List<string>();

            existingDepartment.Users = await _userManager.Users
                .Where(u => selectedUserIds.Contains(u.Id))
                .ToListAsync();

            await _departmentService.UpdateDepartmentAsync(existingDepartment);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return View("NotFound");
            }

            await _departmentService.DeleteDepartmentAsync(id);
            return RedirectToAction("Index");
        }

        private async Task PopulateViewBagsForDepartment(string selectedManagerId = null)
        {
            var managers = await _userManager.GetUsersInRoleAsync("DepartmentManager");
            var allUsers = await _userManager.Users.ToListAsync();

            ViewBag.Managers = new SelectList(managers, "Id", "FullName", selectedManagerId);
            ViewBag.AllUsers = allUsers;
        }
    }
}