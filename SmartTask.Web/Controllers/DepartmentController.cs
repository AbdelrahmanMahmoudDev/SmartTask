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


            



        //GetDepartments method is returning the IEnumerable Departments from database
        [HttpGet]
        private IEnumerable<Department> GetDepartments()
        {
            return new List<Department>
            {
                new Department
        {
            Name = "Department1",

        },
        
            };
        }
        [HttpGet]
        public ActionResult GetData()
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
            var location = Request.Query["location"].FirstOrDefault();
            var position = Request.Query["position"].FirstOrDefault();
            var startDateStr = Request.Query["startDate"].FirstOrDefault();
            var endDateStr = Request.Query["endDate"].FirstOrDefault();
            var minSalaryStr = Request.Query["minSalary"].FirstOrDefault();
            var maxSalaryStr = Request.Query["maxSalary"].FirstOrDefault();
            var minAgeStr = Request.Query["minAge"].FirstOrDefault();
            var maxAgeStr = Request.Query["maxAge"].FirstOrDefault();

            var branches = GetDepartments();
            int totalRecords = branches.Count();



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


  