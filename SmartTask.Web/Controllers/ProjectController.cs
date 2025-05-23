﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTask.Bl.IServices;
using Microsoft.EntityFrameworkCore;
using SmartTask.BL.IServices;
using SmartTask.BL.Services;
using SmartTask.Core.Models;
using SmartTask.Web.ViewModels.ProjectVM;

namespace SmartTask.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IDepartmentService _departmentService;
        private readonly IBranchService _branchService;

        public ProjectController(
            IProjectService projectService, IDepartmentService departmentService,
            IBranchService branchService, UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _projectService = projectService;
            _departmentService = departmentService;
            _branchService = branchService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(string searchString, int? selectedDepartmentId, int? selectedBranchId, int page = 1, int pageSize = 10)
        {
            //var projects = await _projectService.GetFilteredProjectsAsync(searchString, page, pageSize);

            //var viewModel = new ProjectIndexViewModel
            //{
            //    Projects = projects,
            //    SearchString = searchString
            //};

            //return View(viewModel);
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var branches = await _branchService.GetAllAsync();
            int x = 5;
            var projects = await _projectService.GetFilteredByDepartmentProjectsAsync(searchString, selectedDepartmentId, selectedBranchId, page, pageSize);

            var viewModel = new ProjectIndexViewModel
            {
                Projects = projects,
                SearchString = searchString,
                SelectedDepartmentId = selectedDepartmentId,
                SelectedBranchId = selectedBranchId,
                Departments = departments.ToList(),
                Branches = branches.ToList()
            };

            return View(viewModel);
        }
        public async Task<IActionResult> FilterIndex(string searchString, int? selectedDepartmentId, int? selectedBranchId, int page = 1, int pageSize = 10)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var branches = await _branchService.GetAllAsync();

            var projects = await _projectService.GetFilteredByDepartmentProjectsAsync(searchString, selectedDepartmentId, selectedBranchId, page, pageSize);

            var viewModel = new ProjectIndexViewModel
            {
                Projects = projects,
                SearchString = searchString,
                SelectedDepartmentId = selectedDepartmentId,
                SelectedBranchId = selectedBranchId,
                Departments = departments.ToList(),
                Branches = branches.ToList()
            };

            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var admins = await _userManager.GetUsersInRoleAsync("Admin");
            //ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName");
            ViewBag.departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.branches = await _branchService.GetAllAsync();


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectFormViewModel model)
        {
            //var admins = await _userManager.GetUsersInRoleAsync("Admin");
            //ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName");
            ViewBag.departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.branches = await _branchService.GetAllAsync();

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
                CreatedById = currentUser.Id,
                DepartmentId = model.SelectedDepartmentId,
                BranchId = model.SelectedBranchId
            };

            if (model.SelectedBranchId.HasValue && model.SelectedDepartmentId.HasValue)
            {
                var usersInBranchAndDepartment = await _userManager.Users
                    .Where(u => u.BranchId == model.SelectedBranchId && u.DepartmentId == model.SelectedDepartmentId)
                    .ToListAsync();

                project.ProjectMembers = new List<ProjectMember>();

                foreach (var member in usersInBranchAndDepartment)
                {
                    project.ProjectMembers.Add(new ProjectMember
                    {
                        UserId = member.Id
                    });
                }
            }

            await _projectService.AddProjectAsync(project);

            //SignalR Part
            string notificationType = "newproject";
            string notificationMessage = $"You have been assigned as the owner of a new project by {currentUser.FullName}: {project.Name}.";
            List<string> user = new List<string>() { model.OwnerId};
            await _notificationService.sendSignalRNotificationAsync(user, model.OwnerId, notificationType, notificationMessage,project.Id);

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

            //SignalR Part
            var currentUser = await _userManager.GetUserAsync(User);
            string notificationType = "deleteproject";
            string notificationMessage = $"The project You have been assigned as the owner was deleted by {currentUser.FullName}: {project.Name}.";
            List<string> user = new List<string>() { project.OwnerId };
            await _notificationService.sendSignalRNotificationAsync(user, project.OwnerId, notificationType, notificationMessage,id);
            
            // remove from db
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
            ViewBag.departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.branches = await _branchService.GetAllAsync();
            //var admins = await _userManager.GetUsersInRoleAsync("Admin");
            //ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName");

            if (project == null)
            {
                return NotFound();
            }

            var currentUserIds = project.ProjectMembers.Select(pm => pm.UserId).ToList();

            var allUsers = await _userManager.Users.ToListAsync();

            ViewBag.AllUsers = allUsers.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.FullName,
                Selected = currentUserIds.Contains(u.Id)
            }).ToList();

            var nonAssignedUsers = allUsers.Where(u => !currentUserIds.Contains(u.Id)).ToList();


            ViewBag.NonAssignedUsers = new SelectList(nonAssignedUsers, "Id", "FullName");

            var model = new ProjectEditViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                Status = project.Status,
                SelectedDepartmentId = project.DepartmentId,
                SelectedBranchId = project.BranchId,
                AssignedUsers = project.ProjectMembers.Select(pm => new UserCheckboxModel
                {
                    UserId = pm.UserId,
                    FullName = pm.User.FullName,
                    IsChecked = true
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProjectEditViewModel model, List<string> AssignedUserIds)
        {
            ViewBag.departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.branches = await _branchService.GetAllAsync();
            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }

            if (!ModelState.IsValid)
            {
             //   var admins = await _userManager.GetUsersInRoleAsync("Admin");
               // ViewBag.AdminUsers = new SelectList(admins, "Id", "FullName");
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
            project.Status = model.Status;
            project.DepartmentId = model.SelectedDepartmentId;
            project.BranchId = model.SelectedBranchId;

            var updatedMembers = AssignedUserIds.Select(userId => new ProjectMember
            {
                ProjectId = project.Id,
                UserId = userId
            }).ToList();

            project.ProjectMembers.Clear();
            foreach (var member in updatedMembers)
            {
                project.ProjectMembers.Add(member);
            }

            await _projectService.UpdateProjectAsync(project);

            //SignalR
            var currentUser = await _userManager.GetUserAsync(User);
            string notificationType = "updateproject";
            string notificationMessage = $"The project You have been assigned as the owner was updated by {currentUser.FullName}: {project.Name}.";
            List<string> user = new List<string>() { project.OwnerId };
            await _notificationService.sendSignalRNotificationAsync(user, project.OwnerId, notificationType, notificationMessage,project.Id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AssignUser(int projectId)
        {
            var project = await _projectService.GetProjectByIdAsync(projectId);
            if (project == null) return NotFound();

            var projectMemberIds = project.ProjectMembers.Select(pm => pm.UserId).ToList();

            var users = await _userManager.Users
                .Where(u => !projectMemberIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.ProjectId = projectId;
            ViewBag.Users = new SelectList(users, "Id", "FullName");

            return View(projectId);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUser(int projectId, List<string> selectedUserIds)
        {
            foreach (var userId in selectedUserIds)
            {
                await _projectService.AddMemberAsync(projectId, userId);
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> GetUsersByBranchAndDepartment(int branchId, int departmentId)
        {
            var users = await _userManager.Users
                .Where(u => u.BranchId == branchId && u.DepartmentId == departmentId)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            return Json(users);
        }
    }
}
