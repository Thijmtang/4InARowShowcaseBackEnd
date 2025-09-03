using System.Data;
using System.Security.Claims;
using System.Text;
using DotNetAuth.Models;
using DotNetAuth.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAuth.Controllers
{
    [Route("api/auth")]
    [Authorize]
    [ApiController]
    public class AuthenticatorController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager;
        private readonly AuthService _authService;

        public AuthenticatorController(UserManager<IdentityUser> userManager, AuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            // User does not exist
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            // Invalid credentials
            if (!result)
            {
                return BadRequest();
            }

            // User has enabled two factor authentication, we need to do addition checks
            if (user.TwoFactorEnabled)
            {
                if (model.TwoFactorCode.IsNullOrEmpty() )
                {
                    return BadRequest(new { detail = "RequiresTwoFactor" });
                }

                var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                    user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.TwoFactorCode);

                // Invalid 2FA code
                if (!is2faTokenValid && user.TwoFactorEnabled)
                {
                    return BadRequest();
                }
            }

            var userDto = new User(
                Id: Guid.Parse(user.Id),
                Username: user.UserName,
                Email: user.Email,
                Roles: await _userManager.GetRolesAsync(user)
            );

            var token = await _authService.CreateJwt(userDto);

            return Ok(token);
        }

        /// <summary>
        /// Route which utilises the ASP.NET Core Identity Two Factor Authentication, to see if the token is still valid
        /// </summary>
        [HttpGet("token/verify")]
        [Authorize]
        public async Task<IActionResult> VerifyToken()
        {
            var user = await _userManager.GetUserAsync(User);

            var userDto = new User(
                Id: Guid.Parse(user.Id),
                Username: user.UserName,
                Email: user.Email,
                Roles: await _userManager.GetRolesAsync(user)
            );

            var token = await _authService.CreateJwt(userDto);

            return Ok(token);
        }
    }
}