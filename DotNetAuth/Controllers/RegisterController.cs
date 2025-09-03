using DotNetAuth.Models;
using DotNetAuth.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAuth.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager;
        private readonly AuthService _authservice;

        public RegisterController(UserManager<IdentityUser> userManager, AuthService authservice)
        {
            _userManager = userManager;
            _authservice = authservice;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser() { UserName = model.Username, Email = model.Email };


            var result = await _userManager.CreateAsync(user, model.Password);
            
   
            
            if (result.Succeeded == false)
            {
                return BadRequest();
            }

            user = await _userManager.FindByEmailAsync(model.Email);
                
            var userDto = new User(
                Id: Guid.Parse(user.Id),
                Username: user.UserName,
                Email: user.Email,
                Roles: await _userManager.GetRolesAsync(user)
            );
            var token = _authservice.CreateJwt(userDto);
                
            return Ok(token);
        }


        
        [Authorize]
        [HttpGet("test")]
        public async Task<IActionResult> test()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles =  await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }
    }
}