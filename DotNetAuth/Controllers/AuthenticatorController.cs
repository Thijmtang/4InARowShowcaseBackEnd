using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAuth.Controllers
{
    [Authorize]
    [ApiController]
    public class AuthenticatorController : ControllerBase
    {
        private UserManager<IdentityUser> _userManager;
        public AuthenticatorController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("signOut")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Clear the existing external cookie
                HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application", new CookieOptions { Secure = true });

                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        
        [HttpPost("isSignedIn"), AllowAnonymous]
        public  IActionResult IsSignedIn()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("hasRole")]
        public IActionResult HasRole(string role)
        {
            if (User.IsInRole(role))
            {
                return Ok();
            }

            return BadRequest();
        }


    }
}
