using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTask.BL.IServices;
using SmartTask.Core.Models;
using SmartTask.Web.Dto;
using System.Linq.Expressions;

namespace SmartTask.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;

        public UserApiController(IUserService userService, IDepartmentService departmentService)
        {
            _userService = userService;
            _departmentService = departmentService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(string? searchString = null, int page = 1, int pageSize = 10)
        {

            var users = await _userService.GetFilteredAsync(searchString, page, pageSize);

            var userDtos = users.Select(u => new GetAllUsersDto
            {
                FullName = u.FullName,
                Email = u.Email,
                DepartmentName = u.Department?.Name
            });
            return Ok(userDtos);
        }
        [HttpGet("without-department")]
        public async Task<IActionResult> GetUsersWithoutDepartment(int page = 1, int pageSize = 10)
        {

            var users = await _userService.GetFilteredAsync(null, page, pageSize);
            var departments = await _departmentService.GetAllDepartmentsAsync();

            var userDtos = users.Select(u => new GetUsersWithoutDepartmentDto
            {
                FullName = u.FullName,
                Email = u.Email,
            });
            
            return Ok(new
            {
                Users = userDtos
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID is required");

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var userDto = new GetAllUsersDto
            {
                FullName = user.FullName,
                Email = user.Email,
                DepartmentName = user.Department?.Name
            };
            return Ok(userDto);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignToDepartment([FromBody] AssignUserDepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.AssignUserToDepartmentAsync(dto.userId, dto.DepartmentId);

            if (!success)
                return BadRequest("Failed to assign user to department.");

            return Ok(new { Message = "User assigned successfully." });
           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null) return NotFound("User Not found");            

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.UserName;
            user.DepartmentId = dto.DepartmentId;

            var success = await _userService.UpdateAsync(user);
            if (!success)
                return NotFound("Update failed.");

            return Ok(new { Message = "User updated successfully." , user  = dto});
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userService.DeleteAsync(id);
            return Ok(new { Message = "User deleted successfully." , user = user});
        }

    }
}
