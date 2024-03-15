using DotNetAuth.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public RegisterController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [AllowAnonymous] 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = new IdentityUser() { UserName = model.Username, Email = model.Email };
            
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Your code for successful registration
                return Ok("User registered successfully");
            }


            return BadRequest();
        }
    }
}
