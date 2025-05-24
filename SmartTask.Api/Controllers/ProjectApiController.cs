using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTask.Api.DTOs;
using SmartTask.Api.DTOs.ProjectDto;
using SmartTask.Bl.Helpers;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using System.Security.Claims;

namespace SmartTask.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectApiController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectApiController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetProjects(
            [FromQuery] string searchString = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedProjects = await _projectService.GetFilteredProjectsAsync(searchString, page, pageSize);

            var projectDtos = paginatedProjects.Items.Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                BranchId = p.BranchId,
                DepartmentId = p.DepartmentId,
                OwnerId = p.OwnerId,
                Owner = p.Owner != null ? new OwnerDTO
                {
                    FullName = p.Owner.FullName,
                } : null
            }).ToList();

            var result = new
            {
                PageIndex = paginatedProjects.PageIndex,
                TotalPages = paginatedProjects.TotalPages,
                TotalCount = paginatedProjects.TotalCount,
                PageSize = paginatedProjects.PageSize,
                Items = projectDtos
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
                return NotFound();

            var projectDto = new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                BranchId = project.BranchId,
                DepartmentId = project.DepartmentId,

                Owner = new OwnerDTO
                {
                    FullName = project.Owner?.FullName,
                }
            };


            return Ok(projectDto);
        }

        [HttpPost("CreateProject")]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO projectDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var project = new Project
            {
                Name = projectDTO.Name,
                Description = projectDTO.Description,
                StartDate = projectDTO.StartDate,
                EndDate = projectDTO.EndDate,
                Status = string.IsNullOrWhiteSpace(projectDTO.Status) ? "Pending" : projectDTO.Status,
                DepartmentId = projectDTO.DepartmentId,
                BranchId = projectDTO.BranchId,
                OwnerId = projectDTO.OwnerId,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow
            };

            var newProject = await _projectService.AddProjectAsync(project);

            var resultDto = new ProjectDTO
            {
                Id = newProject.Id,
                Name = newProject.Name,
                Description = newProject.Description,
                StartDate = newProject.StartDate,
                EndDate = newProject.EndDate,
                Status = newProject.Status,
                DepartmentId = newProject.DepartmentId,
                BranchId = newProject.BranchId,
                OwnerId = newProject.OwnerId,
                Owner = newProject.Owner != null ? new OwnerDTO { FullName = newProject.Owner.FullName } : null
            };

            return CreatedAtAction(nameof(GetProject), new { id = newProject.Id }, new
            {
                success = true,
                message = "Project created successfully",
                data = resultDto
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDTO projectDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

            var existingProject = await _projectService.GetProjectByIdAsync(id);
            if (existingProject == null)
                return NotFound(new { success = false, message = "Project not found" });

            existingProject.Name = projectDTO.Name;
            existingProject.Description = projectDTO.Description;
            existingProject.StartDate = projectDTO.StartDate;
            existingProject.EndDate = projectDTO.EndDate;
            existingProject.Status = projectDTO.Status;
            existingProject.DepartmentId = projectDTO.DepartmentId;
            existingProject.BranchId = projectDTO.BranchId;
            existingProject.OwnerId = projectDTO.OwnerId;
            existingProject.UpdatedAt = DateTime.UtcNow;

            await _projectService.UpdateProjectAsync(existingProject);

            return Ok(new
            {
                success = true,
                message = "Project updated successfully"
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
                return NotFound(new
                {
                    success = false,
                    message = "Project not found"
                });

            await _projectService.DeleteProjectAsync(id);

            return Ok(new
            {
                success = true,
                message = "Project deleted successfully"
            });
        }

    }
}