using System.ComponentModel;
using System.Globalization;
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


        //GetBranchs method is returning the IEnumerable Branchs from database
        [HttpGet]
        IEnumerable<Branch> GetBranchs()
        {
            return new List<Branch>
            {
                new Branch
        {
            Name = "Branch1",

        },
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetData()
        {

            // Get request parameters from DataTables
            var draw = Request.Query["draw"].FirstOrDefault();
            var start = Request.Query["start"].FirstOrDefault();
            var length = Request.Query["length"].FirstOrDefault();
            var searchValue = Request.Query["search[value]"].FirstOrDefault();
            var sortColumnIndex = Request.Query["order[0][column]"].FirstOrDefault();
            var sortDirection = Request.Query["order[0][dir]"].FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;


            // Advanced filter parameters
            var name = Request.Query["name"].FirstOrDefault();
            var manager = Request.Query["manager"].FirstOrDefault();
            //var startDateStr = Request.Query["startDate"].FirstOrDefault();
            //var endDateStr = Request.Query["endDate"].FirstOrDefault();
            //var minSalaryStr = Request.Query["minSalary"].FirstOrDefault();
            //var maxSalaryStr = Request.Query["maxSalary"].FirstOrDefault();
            //var minAgeStr = Request.Query["minAge"].FirstOrDefault();
            //var maxAgeStr = Request.Query["maxAge"].FirstOrDefault();

            //DateTime? startDate = null;
            //DateTime? endDate = null;
            //int? minSalary = null;
            //int? maxSalary = null;
            //int? minAge = null;
            //int? maxAge = null;

            //if (!string.IsNullOrEmpty(startDateStr))
            //{
            //    if (DateTime.TryParseExact(startDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate))
            //    {
            //        startDate = parsedStartDate;
            //    }
            //}

            //if (!string.IsNullOrEmpty(endDateStr))
            //{
            //    if (DateTime.TryParseExact(endDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedEndDate))
            //    {
            //        endDate = parsedEndDate.AddDays(1).AddSeconds(-1); // End of day
            //    }
            //}

            //if (!string.IsNullOrEmpty(minSalaryStr) && int.TryParse(minSalaryStr, out int parsedMinSalary))
            //{
            //    minSalary = parsedMinSalary;
            //}

            //if (!string.IsNullOrEmpty(maxSalaryStr) && int.TryParse(maxSalaryStr, out int parsedMaxSalary))
            //{
            //    maxSalary = parsedMaxSalary;
            //}

            //if (!string.IsNullOrEmpty(minAgeStr) && int.TryParse(minAgeStr, out int parsedMinAge))
            //{
            //    minAge = parsedMinAge;
            //}

            //if (!string.IsNullOrEmpty(maxAgeStr) && int.TryParse(maxAgeStr, out int parsedMaxAge))
            //{
            //    maxAge = parsedMaxAge;
            //}

            var branches = GetBranchs();
            int totalRecords = branches.Count();

            if (!string.IsNullOrEmpty(name))
            {
                branches = branches.Where(x => x.Name == name).ToList();
            }

            if (!string.IsNullOrEmpty(manager))
            {
                branches = branches.Where(x => x.ManagerId == manager).ToList();
            }

            //if (startDate.HasValue)
            //{
            //    employees = employees.Where(x => x.StartDate >= startDate.Value).ToList();
            //}

            //if (endDate.HasValue)
            //{
            //    employees = employees.Where(x => x.StartDate <= endDate.Value).ToList();
            //}

            //if (minSalary.HasValue)
            //{
            //    employees = employees.Where(x => x.Salary >= minSalary.Value).ToList();
            //}

            //if (maxSalary.HasValue)
            //{
            //    employees = employees.Where(x => x.Salary <= maxSalary.Value).ToList();
            //}

            //if (minAge.HasValue)
            //{
            //    employees = employees.Where(x => x.Age >= minAge.Value).ToList();
            //}

            //if (maxAge.HasValue)
            //{
            //    employees = employees.Where(x => x.Age <= maxAge.Value).ToList();
            //}



            //Filter(Search)
            if (!string.IsNullOrEmpty(searchValue))
            {
                branches = branches.Where(x => x.Name.ToLower().Contains(searchValue.ToLower())).ToList();
            }

            // Total records after filtering
            int totalRecordsFiltered = branches.Count();

            // Sorting
            if (!string.IsNullOrEmpty(sortColumnIndex) && !string.IsNullOrEmpty(sortDirection))
            {
                switch (Convert.ToInt32(sortColumnIndex))
                {
                    case 0:
                        branches = sortDirection == "asc" ? branches.OrderBy(e => e.Name) : branches.OrderByDescending(e => e.Name);
                        break;
                    default:
                        branches = branches.OrderBy(e => e.Name);
                        break;
                }
            }



            //Pagination 
            if (pageSize > 0)
            {
                branches = branches.Skip(skip).Take(pageSize);
            }



            // Format the data for output
            var result = branches.Select(b => new
            {
                b.Id,
                b.Name,
                ManagerName = b.Manager != null ? b.Manager.FullName : "N/A"
            }).ToList();

            // Return JSON data for DataTable
            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecordsFiltered,
                data = result
            });
        }

        
    }
}

