using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTask.BL.IServices;
using SmartTask.BL.Services;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels.ProjectVM;
using SmartTask.Web.Views.Project;

namespace SmartTask.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(
            IProjectService projectService,
            UserManager<ApplicationUser> userManager)
        {
            _projectService = projectService;
            _userManager = userManager;
        }

        public async Task< IActionResult >Index(string searchString, int page = 1, int pageSize = 10)
        {
            var projects = await _projectService.GetFilteredProjectsAsync(searchString, page, pageSize);

            var viewModel = new ProjectIndexViewModel
            {
                Projects = projects,
                SearchString = searchString
            };

            return View(viewModel);
        }
        public async Task< IActionResult >FilterIndex(string searchString, int page = 1, int pageSize = 10)
        {
            var projects = await _projectService.GetFilteredProjectsAsync(searchString, page, pageSize);

            var viewModel = new ProjectIndexViewModel
            {
                Projects = projects,
                SearchString = searchString
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectFormViewModel model)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName");

            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var project = new Project
            {
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                OwnerId = model.OwnerId, 
                CreatedById = currentUser.Id 
            };

            await _projectService.AddProjectAsync(project);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return View("NotFound");
            }

            await _projectService.DeleteProjectAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName"); // Using ViewBag

            var model = new ProjectEditViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                Status = project.Status // Mapping Status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProjectEditViewModel model)
        {
            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }

            if (!ModelState.IsValid)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName"); 
                return View(model);
            }

            var project = await _projectService.GetProjectByIdAsync(model.Id);
            if (project == null)
            {
                return NotFound();
            }

            project.Name = model.Name;
            project.Description = model.Description;
            project.StartDate = model.StartDate;
            project.EndDate = model.EndDate;
            project.OwnerId = model.OwnerId;
            project.Status = model.Status; // تعيين الـ Status

            await _projectService.UpdateProjectAsync(project);
            return RedirectToAction("Index");
        }
    }
}
