using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTask.Bl.IServices;
using SmartTask.BL.IServices;
using SmartTask.BL.Services;
using SmartTask.Core.IRepositories;
using SmartTask.Core.Models;
using SmartTask.Core.Models.BasePermission;
using SmartTask.Web.ViewModels.BranchVM;

namespace SmartTask.Web.Controllers
{
    public class BranchController : Controller
    {
        private readonly IBranchService branchService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDepartmentService departmentService;

        public BranchController(IBranchService branchService,RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IDepartmentService departmentService
            )
        {
            this.branchService = branchService;
            this.userManager = userManager;
            this.departmentService = departmentService;
        }

        [Authorize]
        [DisplayName("Branches name")]
        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 5)
        {
            var branches = await branchService.GetFiltered(searchString, null, page, pageSize);

            var managers = await userManager.GetUsersInRoleAsync("BranchManager");
            

            var viewModel = new BranchIndexViewModel
            {
                Branches = branches,
                SearchString = searchString,
                Managers = new SelectList(managers, "Id", "FullName")
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddBranch()
        {
            var managers = await userManager.GetUsersInRoleAsync("BranchManager");
            var departments = await departmentService.GetAllDepartmentsAsync() ?? new List<Department>();

            ViewBag.Managers = new SelectList(managers ?? new List<ApplicationUser>(), "Id", "UserName");
            ViewBag.Departments = new MultiSelectList(departments, "Id", "Name");

            return View(new BranchFormViewModel
            {
                AllDepartments = departments
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBranch(BranchFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var managers = await userManager.GetUsersInRoleAsync("BranchManager");
                var departments = await departmentService.GetAllDepartmentsAsync();

                ViewBag.Managers = new SelectList(managers, "Id", "UserName");
                ViewBag.Departments = new MultiSelectList(departments, "Id", "Name");

                return View("AddBranch", model);
            }

            var branch = new Branch
            {
                Name = model.Name,
                ManagerId = model.ManagerId,
                BranchDepartments = model.SelectedDepartmentIds?.Select(id => new BranchDepartment
                {
                    DepartmentId = id
                }).ToList()
            };

            await branchService.AddAsync(branch);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var branch = await branchService.GetBranchAsync(id);
            if(branch == null) return View("NotFound");

            await branchService.DeleteAsync(id);
            return RedirectToAction("Index");

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await branchService.GetBranchWithDetailsAsync(id); 
            if (branch == null)
            {
                return View("NotFound");
            }

            var managers = await userManager.GetUsersInRoleAsync("BranchManager");
            var departments = await departmentService.GetAllDepartmentsAsync();

            ViewBag.Roles = new SelectList(managers, "Id", "UserName", branch.ManagerId);

            var model = new BranchFormViewModel
            {
                Id = branch.Id,
                Name = branch.Name,
                ManagerId = branch.ManagerId,
                SelectedDepartmentIds = branch.BranchDepartments.Select(bd => bd.DepartmentId).ToList(),
                AllDepartments = departments
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(BranchFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var managers = await userManager.GetUsersInRoleAsync("BranchManager");
                var departments = await departmentService.GetAllDepartmentsAsync();

                ViewBag.Roles = new SelectList(managers, "Id", "UserName", model.ManagerId);
                model.AllDepartments = departments;

                return View(model);
            }

            var branch = new Branch
            {
                Id = model.Id,
                Name = model.Name,
                ManagerId = model.ManagerId,
                BranchDepartments = model.SelectedDepartmentIds?.Select(id => new BranchDepartment
                {
                    BranchId = model.Id,
                    DepartmentId = id
                }).ToList()
            };

            await branchService.UpdateAsync(branch);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var branch = await branchService.GetBranchWithDetailsAsync(id);
            if (branch == null) return View("NotFound");

            return View(branch);
        }
    }
}
