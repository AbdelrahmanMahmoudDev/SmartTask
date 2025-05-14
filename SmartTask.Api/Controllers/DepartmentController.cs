using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartTask.Api.DTOs.Department;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;

namespace SmartTask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentController(IDepartmentService departmentService, UserManager<ApplicationUser> userManager)
        {
            _departmentService = departmentService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments([FromQuery] string searchString, [FromQuery] int page = 1, [FromQuery] int pageSize = 4)
        {
            var departments = await _departmentService.GetFilteredDepartments(searchString, page, pageSize);
            var result = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                ManagerId = d.ManagerId
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDetailsDto>> GetDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentWithDetailsAsync(id);
            if (department == null) return NotFound();

            var dto = new DepartmentDetailsDto
            {
                Id = department.Id,
                Name = department.Name,
                ManagerName = department.Manager?.FullName ?? "No Manager Assigned",
                Users = department.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentCreateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditDepartment(int id, [FromBody] DepartmentUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();

            department.Name = model.Name;
            department.ManagerId = model.ManagerId;
            department.Users = await _userManager.Users
                .Where(u => model.SelectedUserIds.Contains(u.Id))
                .ToListAsync();

            await _departmentService.UpdateDepartmentAsync(department);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null) return NotFound();

            await _departmentService.DeleteDepartmentAsync(id);
            return NoContent();
        }
    }
}
