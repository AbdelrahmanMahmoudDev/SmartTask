using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

            var user = await userManager.GetUserAsync(User);

            var roles = await userManager.GetRolesAsync(user);

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
            if (branch == null) return View("NotFound");

            var managers = await userManager.GetUsersInRoleAsync("BranchManager");

            var currentUsers = await userManager.Users
                .Where(u => u.BranchId == id)
                .ToListAsync();

            var availableUsers = await userManager.Users
                .Where(u => u.BranchId == null)
                .ToListAsync();

            var allUsers = currentUsers.Union(availableUsers).ToList();

            ViewBag.Managers = new SelectList(managers, "Id", "FullName", branch.ManagerId);
            ViewBag.Users = new MultiSelectList(allUsers, "Id", "FullName", currentUsers.Select(u => u.Id));

            var model = new BranchFormViewModel
            {
                Id = branch.Id,
                Name = branch.Name,
                ManagerId = branch.ManagerId,
                SelectedDepartmentIds = branch.BranchDepartments.Select(bd => bd.DepartmentId).ToList(),
                SelectedUserIds = currentUsers.Select(u => u.Id).ToList(),
                AllDepartments = await departmentService.GetAllDepartmentsAsync()
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
                ViewBag.Managers = new SelectList(managers, "Id", "FullName", model.ManagerId);
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

            var existingUsers = await userManager.Users
                .Where(u => u.BranchId == model.Id)
                .ToListAsync();

            foreach (var user in existingUsers)
            {
                if (!model.SelectedUserIds.Contains(user.Id))
                {
                    user.BranchId = null;
                    await userManager.UpdateAsync(user);
                }
            }

            foreach (var userId in model.SelectedUserIds)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null && user.BranchId != model.Id)
                {
                    user.BranchId = model.Id;
                    await userManager.UpdateAsync(user);
                }
            }

            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var branch = await branchService.GetBranchWithDetailsAsync(id);
            if (branch == null) return View("NotFound");

            var usersInBranch = await userManager.Users
                                    .Where(u => u.BranchId == id)
                                    .ToListAsync();

            ViewBag.UsersInBranch = usersInBranch;
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
            Name = "Ahmed Ali",

        },
        new Branch
        {
            Name = "Mona Hassan",

        },
        new Branch
        {
            Name = "Mohamed Fathy",

        },
        new Branch
        {
            Name = "Fatima Ibrahim",

        },
        new Branch
        {
            Name = "Youssef Tarek",

        },
        new Branch
        {
            Name = "Sara Khaled",

        },
        new Branch
        {
            Name = "Omar Mostafa",

        },
        new Branch
        {
            Name = "Nourhan Adel",

        },
        new Branch
        {
            Name = "Khaled Samir",

        },
        new Branch
        {
            Name = "Salma Ramadan",

        },
        new Branch
        {
            Name = "Ali Mahmoud",

        },
        new Branch
        {
            Name = "Laila ElKady",

        },
        new Branch
        {
            Name = "Amr Tamer",

        },
        new Branch
        {
            Name = "Hala Nasser",

        }, new Branch
        {
            Name = "Marwa sayed",

        },
        new Branch
        {
            Name = "Fady Khalil",

        }

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
            var location = Request.Query["location"].FirstOrDefault();
            var position = Request.Query["position"].FirstOrDefault();
            var startDateStr = Request.Query["startDate"].FirstOrDefault();
            var endDateStr = Request.Query["endDate"].FirstOrDefault();
            var minSalaryStr = Request.Query["minSalary"].FirstOrDefault();
            var maxSalaryStr = Request.Query["maxSalary"].FirstOrDefault();
            var minAgeStr = Request.Query["minAge"].FirstOrDefault();
            var maxAgeStr = Request.Query["maxAge"].FirstOrDefault();

            var branches = GetBranchs();
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


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AssignUsers(int branchId)
        {
            var branch = await branchService.GetBranchWithDetailsAsync(branchId);
            if (branch == null)
                return View("NotFound");

            var usersWithOutBranch = await userManager.Users
                .Where(u => u.BranchId == null)
                .ToListAsync();

            ViewBag.Users = new MultiSelectList(usersWithOutBranch, "Id", "FullName");

            var viewModel = new AssignUsersToBranchViewModel
            {
                BranchId = branchId,
                SelectedUserIds = new List<string>() 
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AssignUsers(AssignUsersToBranchViewModel model)
        {
            var branch = await branchService.GetBranchWithDetailsAsync(model.BranchId);
            if (branch == null)
                return View("NotFound");

            var selectedUsers = await userManager.Users
                .Where(u => model.SelectedUserIds.Contains(u.Id))
                .ToListAsync();

            foreach (var user in selectedUsers)
            {
                user.BranchId = branch.Id;
                await userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }

    }
}

