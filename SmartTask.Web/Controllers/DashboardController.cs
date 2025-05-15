using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartTask.BL.Services;

namespace SmartTask.Web.Controllers
{
    [Authorize] // Requires users to be logged in
    public class DashboardController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDepartmentService _departmentService;
        private const int PageSize = 10;

        public DashboardController(
            IProjectService projectService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IDepartmentService departmentService)
        {
            _projectService = projectService;
            _userManager = userManager;
            _roleManager = roleManager;
            _departmentService = departmentService;
        }

        // GET: Dashboard - Main dashboard page
        public async Task<IActionResult> Index(string searchString, int? departmentId, int page = 1)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get departments for filter dropdown
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            ViewBag.SelectedDepartment = departmentId;
            ViewBag.SearchString = searchString;

            // Get projects based on authorization
            if (User.IsInRole("Admin"))
            {
                // Admin sees all projects with pagination and filtering
                var projects = await _projectService.GetFilteredProjectsAsync(searchString, departmentId, page, PageSize);
                return View(projects);
            }
            else
            {
                // Regular users and Project Managers see their own projects
                var userProjects = await _projectService.GetUserProjectsAsync(currentUser.Id);

                // Apply filters
                if (!string.IsNullOrEmpty(searchString))
                {
                    userProjects = userProjects
                        .Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString))
                        .ToList();
                }

                if (departmentId.HasValue)
                {
                    userProjects = userProjects
                        .Where(p => p.DepartmentId == departmentId.Value)
                        .ToList();
                }

                // Manual pagination for filtered user projects
                var totalCount = userProjects.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
                var paginatedProjects = userProjects
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.HasPreviousPage = page > 1;
                ViewBag.HasNextPage = page < totalPages;

                return View("UserProjects", paginatedProjects);
            }
        }

        // GET: Dashboard/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            Project project;

            if (User.IsInRole("Admin"))
            {
                // Admin can see any project
                project = await _projectService.GetProjectByIdAsync(id);
            }
            else
            {
                // Regular users and Project Managers need to be associated with the project
                project = await _projectService.GetProjectDetailsAsync(id, currentUser.Id);
            }

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Dashboard/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Only Admin can create new projects
            return View();
        }

        // POST: Dashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,Status,DepartmentId,BranchId,OwnerId")] Project project)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Set creation metadata
            project.CreatedById = currentUser.Id;
            project.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                await _projectService.AddProjectAsync(project);

                // Add owner as project member if owner is specified
                if (!string.IsNullOrEmpty(project.OwnerId))
                {
                    await _projectService.AddMemberAsync(project.Id, project.OwnerId);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Dashboard/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can edit
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Dashboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerId,Name,Description,StartDate,EndDate,Status,DepartmentId,BranchId")] Project project)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (id != project.Id)
            {
                return NotFound();
            }

            // Get the original project for authorization check
            var originalProject = await _projectService.GetProjectByIdAsync(id);
            if (originalProject == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can update
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (originalProject.OwnerId == currentUser.Id ||
                   originalProject.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Preserve original values for fields we don't want to update
                project.CreatedById = originalProject.CreatedById;
                project.CreatedAt = originalProject.CreatedAt;
                project.UpdatedAt = DateTime.Now;

                // Update the project
                await _projectService.UpdateProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Dashboard/FilterByDepartment
        public async Task<IActionResult> FilterByDepartment(int departmentId, string searchString = "", int page = 1)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get departments for filter dropdown
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", departmentId);
            ViewBag.SelectedDepartment = departmentId;
            ViewBag.SearchString = searchString;

            // Get projects based on authorization
            if (User.IsInRole("Admin"))
            {
                // Admin sees all projects for the department with pagination
                var projects = await _projectService.GetFilteredProjectsAsync(searchString, departmentId, page, PageSize);
                return View("Index", projects);
            }
            else
            {
                // Regular users and Project Managers see their own projects for the department
                var userProjects = await _projectService.GetUserProjectsAsync(currentUser.Id);

                // Filter by department and search string
                userProjects = userProjects
                    .Where(p => p.DepartmentId == departmentId)
                    .ToList();

                if (!string.IsNullOrEmpty(searchString))
                {
                    userProjects = userProjects
                        .Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString))
                        .ToList();
                }

                // Manual pagination for filtered user projects
                var totalCount = userProjects.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
                var paginatedProjects = userProjects
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.HasPreviousPage = page > 1;
                ViewBag.HasNextPage = page < totalPages;

                return View("UserProjects", paginatedProjects);
            }
        }

        // GET: Dashboard/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can delete
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Dashboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can delete
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            await _projectService.DeleteProjectAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Dashboard/AddMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> AddMember(int projectId, string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var project = await _projectService.GetProjectByIdAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            // Authorization check: Only Admin or Project Manager who owns the project can add members
            if (!User.IsInRole("Admin") &&
                !(User.IsInRole("ProjectManager") &&
                  (project.OwnerId == currentUser.Id ||
                   project.ProjectMembers.Any(pm => pm.UserId == currentUser.Id))))
            {
                return Forbid();
            }

            var success = await _projectService.AddMemberAsync(projectId, userId);
            if (!success)
            {
                return BadRequest("Failed to add member to the project.");
            }

            return RedirectToAction(nameof(Details), new { id = projectId });
        }
    }
}
