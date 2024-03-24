using DotNetAuth.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace DotNetAuth.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {


        private RoleManager<IdentityRole> _roleManager;
        private UserManager<IdentityUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        [HttpPut("Edit")]
        public async Task<IActionResult> EditRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            bool editable = !await _userManager.IsInRoleAsync(user, "Admin");
            
            // Cannot edit a admin user, as an admin @todo add superadmin
            if (!editable)
            {
                return BadRequest();
            }

            await _userManager.AddToRoleAsync(user, role);
            // var roles = _roleManager.;
            return Ok();
        }
    }
}
