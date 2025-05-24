using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Data;
using System.Threading.Tasks;

namespace SmartTask.Web.Controllers
{
    public class ManagerController : Controller
    {
        private readonly SmartTaskContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerController(
            SmartTaskContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> DepartmentManagerProjects()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var isManager = await _context.Departments.AnyAsync(d => d.ManagerId == currentUser.Id);

            if (!isManager)
            {
                return NotFound("You are not a manager of any department");
            }

            var projects = await _context.Projects.Where(p => p.Department.ManagerId == currentUser.Id).ToListAsync();

            return View(projects);
        }

        public async Task<IActionResult> BranchManagerProjects()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.ManagerId == currentUser.Id);

            if (branch == null)
            {
                return NotFound("You are not a manager of any branch");
            }

            var departmentIds = await _context.BranchDepartments.Where(bd => bd.BranchId == branch.Id).Select(bd => bd.DepartmentId).ToListAsync();

            var projects = await _context.Projects.Where(p => p.DepartmentId.HasValue && departmentIds.Contains(p.DepartmentId.Value)).Include(C => C.Department).Include(T => T.Tasks).ToListAsync();

            return View(projects);
        }
    }
}