using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartTask.Api.DTOs.BranchDto;
using SmartTask.Api.DTOs.Department;
using SmartTask.Bl.IServices;
using SmartTask.Bl.Services;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;

namespace SmartTask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDepartmentService _departmentService;

        public BranchesController(
            IBranchService branchService,
            UserManager<ApplicationUser> userManager,
            IDepartmentService departmentService)
        {
            _branchService = branchService;
            _userManager = userManager;
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBranches([FromQuery] string? searchString, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var branches = await _branchService.GetFiltered(searchString, null,  page, pageSize);

            var result = branches.Select(b => new BranchDto
            {
                Id = b.Id,
                Name = b.Name,
                ManagerId = b.ManagerId,
                DepartmentIds = b.BranchDepartments?.Select(bd => bd.DepartmentId).ToList() ?? new()
            });

            return Ok(new
            {
                TotalCount = branches.TotalCount,
                PageIndex = branches.PageIndex,
                PageSize = branches.PageSize,
                Branches = result
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDetailsDto>> GetBranch(int id)
        {
            var branch = await _branchService.GetBranchWithDetailsAsync(id);
            if (branch == null) return NotFound();

            var departments = await _departmentService.GetAllDepartmentsAsync();

            return new BranchDetailsDto
            {
                Id = branch.Id,
                Name = branch.Name,
                ManagerName = branch.Manager?.FullName ?? "No Manager Assigned",
                Departments = branch.BranchDepartments?
                    .Select(bd => new DepartmentDto
                    {
                        Id = bd.DepartmentId,
                        Name = departments.FirstOrDefault(d => d.Id == bd.DepartmentId)?.Name ?? "Unknown",
                        ManagerId = departments.FirstOrDefault(d => d.Id == bd.DepartmentId)?.ManagerId ?? "Unknown",
                    }).ToList() ?? new()
            };
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromBody] BranchCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var branch = new Branch
            {
                Name = dto.Name,
                ManagerId = dto.ManagerId,
                BranchDepartments = dto.DepartmentIds.Select(id => new BranchDepartment
                {
                    DepartmentId = id
                }).ToList()
            };

            await _branchService.AddAsync(branch);
            return CreatedAtAction(nameof(GetBranch), new { id = branch.Id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] BranchUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var branch = new Branch
            {
                Id = dto.Id,
                Name = dto.Name,
                ManagerId = dto.ManagerId,
                BranchDepartments = dto.DepartmentIds.Select(id => new BranchDepartment
                {
                    DepartmentId = id,
                    BranchId = dto.Id
                }).ToList()
            };

            await _branchService.UpdateAsync(branch);
            return Ok(new { Message = "Update Successful" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _branchService.GetBranchAsync(id);
            if (result == null) return BadRequest("Branch Not Found");

            await _branchService.DeleteAsync(id);
            return Ok("Branch Deleted Successfuly");
        }

        [HttpGet("managers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetBranchManagers()
        {
            var managers = await _userManager.GetUsersInRoleAsync("BranchManager");
            return Ok(managers.Select(m => new UserDto
            {
                Id = m.Id,
                FullName = m.FullName,
                UserName = m.UserName,
                Email = m.Email
            }));
        }
    }
}
