using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTask.Bl.IServices;
using SmartTask.BL.IServices;
using SmartTask.Core.IRepositories;
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
        private readonly IBranchService _branchService;
        private readonly IBranchDepartmentRepository _branchDepartmentRepository;

        public DepartmentController(
            IDepartmentService departmentService,
            UserManager<ApplicationUser> userManager,
            IBranchService branchService,
            IBranchDepartmentRepository branchDepartmentRepository)
        {
            _departmentService = departmentService;
            _userManager = userManager;
            _branchService = branchService;
            _branchDepartmentRepository = branchDepartmentRepository;
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
            var branches = await _branchService.GetAllAsync();
            ViewBag.Branches = branches;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagsForDepartment(model.ManagerId);
                ViewBag.Branches = await _branchService.GetAllAsync();
                return View(model);
            }

            var department = new Department
            {
                Name = model.Name,
                ManagerId = model.ManagerId
            };

            await _departmentService.AddDepartmentAsync(department);

            if (model.SelectedBranchIds != null && model.SelectedBranchIds.Any())
            {
                foreach (var branchId in model.SelectedBranchIds)
                {
                    var branchDepartment = new BranchDepartment
                    {
                        BranchId = branchId,
                        DepartmentId = department.Id
                    };
                    await _branchDepartmentRepository.AddAsync(branchDepartment);
                }
            }

            if (model.SelectedUserIds != null && model.SelectedUserIds.Any())
            {
                var users = await _userManager.Users
                    .Where(u => model.SelectedUserIds.Contains(u.Id) && u.DepartmentId == null)
                    .ToListAsync();

                foreach (var user in users)
                {
                    user.DepartmentId = department.Id;
                    await _userManager.UpdateAsync(user);
                }
            }

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
                }).ToList(),
                Branches = department.BranchDepartments?.Select(bd => new BranchViewModel
                {
                    Id = bd.Branch.Id,
                    Name = bd.Branch.Name
                }).ToList() ?? new List<BranchViewModel>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetDepartmentWithDetailsAsync(id);
            if (department == null)
                return View("NotFound");

            //var managers = await _userManager.GetUsersInRoleAsync("DepartmentManager");
            var managers = await _userManager.Users.ToListAsync();
            var allBranches = await _branchService.GetAllAsync();

            ViewBag.Managers = new SelectList(managers, "Id", "FullName", department.ManagerId);
            ViewBag.Branches = allBranches;

            var selectedBranchIds = department.BranchDepartments.Select(bd => bd.BranchId).ToList();

            var model = new DepartmentFormViewModel
            {
                Id = department.Id,
                Name = department.Name,
                ManagerId = department.ManagerId,
                SelectedUserIds = department.Users.Select(u => u.Id).ToList(),
                SelectedBranchIds = selectedBranchIds
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagsForDepartment(model.ManagerId);
                ViewBag.Branches = await _branchService.GetAllAsync();
                return View(model);
            }

            var existingDepartment = await _departmentService.GetDepartmentWithDetailsAsync(model.Id);
            if (existingDepartment == null)
                return View("NotFound");

            existingDepartment.Name = model.Name;
            existingDepartment.ManagerId = model.ManagerId;

            var existingBranchIds = existingDepartment.BranchDepartments.Select(bd => bd.BranchId).ToList();
            var newBranchIds = model.SelectedBranchIds ?? new List<int>();

            foreach (var bd in existingDepartment.BranchDepartments.ToList())
            {
                if (!newBranchIds.Contains(bd.BranchId))
                    existingDepartment.BranchDepartments.Remove(bd);
            }

            foreach (var branchId in newBranchIds)
            {
                if (!existingBranchIds.Contains(branchId))
                    existingDepartment.BranchDepartments.Add(new BranchDepartment { BranchId = branchId, DepartmentId = existingDepartment.Id });
            }

            var selectedUserIds = model.SelectedUserIds ?? new List<string>();
            existingDepartment.Users = await _userManager.Users
                .Where(u => selectedUserIds.Contains(u.Id) && u.BranchId.HasValue && newBranchIds.Contains(u.BranchId.Value))
                .ToListAsync();

            try
            {
                await _departmentService.UpdateDepartmentAsync(existingDepartment);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PopulateViewBagsForDepartment(model.ManagerId);
                ViewBag.Branches = await _branchService.GetAllAsync();
                return View(model);
            }

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

            foreach (var project in department.Projects)
            {
                project.DepartmentId = null;
            }

            await _departmentService.DeleteDepartmentAsync(id);
            return RedirectToAction("Index");
        }

        private async Task PopulateViewBagsForDepartment(string selectedManagerId = null, Department department = null)
        {
            //var managers = await _userManager.GetUsersInRoleAsync("DepartmentManager");
            var managers = await _userManager.Users.ToListAsync();
            var allUsers = await _userManager.Users.ToListAsync();

            ViewBag.Managers = new SelectList(managers, "Id", "FullName", selectedManagerId);
            ViewBag.AllUsers = new List<ApplicationUser>();
        }


        private IEnumerable<Department> GetDepartments()
        {
            return new List<Department>
        {
            new Department { Id = 1, Name = "IT Department" },
            new Department { Id = 2, Name = "HR Department" },
            new Department { Id = 3, Name = "Finance Department" },
            new Department { Id = 4, Name = "Marketing Department" },
            new Department { Id = 5, Name = "Sales Department" }
        };
        }



        //GetDepartments method is returning the IEnumerable Departments from database

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


        [HttpGet]
        public async Task<IActionResult> GetUsersByBranches([FromQuery] List<int> branchIds, [FromQuery] int? departmentId = null, [FromQuery] List<string> selectedUserIds = null)
        {
            var usersQuery = _userManager.Users
                .Where(u => u.BranchId.HasValue && branchIds.Contains(u.BranchId.Value))
                .Where(u => u.DepartmentId == null || (departmentId.HasValue && u.DepartmentId == departmentId.Value));

            if (selectedUserIds != null && selectedUserIds.Count > 0)
            {
                usersQuery = usersQuery
                    .Union(_userManager.Users.Where(u => selectedUserIds.Contains(u.Id)));
            }

            var users = await usersQuery
                .Select(u => new { id = u.Id, fullName = u.FullName, email = u.Email })
                .Distinct()
                .ToListAsync();

            return Json(users);
        }
    }
}


  