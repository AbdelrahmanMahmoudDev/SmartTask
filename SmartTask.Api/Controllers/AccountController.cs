using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartTask.Api.DTOs;
using SmartTask.Core.Models;
using SmartTask.DataAccess.Repositories;

namespace SmartTask.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByNameAsync(login.UserName);
                if (user != null)
                {
                    bool passCheck = await userManager.CheckPasswordAsync(user, login.Password);
                    if (passCheck)
                    {
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        var roles = await userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }
                        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sdfljsdlfj9o4oieurwew//cv??fdssdrer///???430958dlsjfkjdssdfl||dsf"));
                        SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        JwtSecurityToken jwt = new JwtSecurityToken(
                            issuer: "http://localhost:5086/",
                            audience: "http://localhost:4200/",
                            expires: DateTime.Now.AddDays(1),
                            claims: claims,
                            signingCredentials: signingCredentials
                            );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(jwt),
                            expires = DateTime.Now.AddDays(1)
                        });

                    }
                }
            }
            return BadRequest(new { Error= "User Name or Password are invalid" });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser();
                applicationUser.UserName = register.UserName;
                applicationUser.FullName = register.FName;
                applicationUser.Email = register.Email;
                applicationUser.PhoneNumber = register.PhoneNumber;
                applicationUser.createdAt = DateTime.Now;
                applicationUser.updatedAt = DateTime.Now;
                IdentityResult result = await userManager.CreateAsync(applicationUser, register.Password);
                if (result.Succeeded)
                {
                    return Ok("Registered Successfully");

                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return BadRequest(ModelState);
        }
    }
}
