//
//
// using DotNetAuth.Models;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.Data;
// using Microsoft.AspNetCore.Mvc;
//
// namespace DotNetAuth.Controllers;
//
// [Authorize]
// [Route("[controller]")]
// [ApiController]
// public class LoginController
// {
//     [AllowAnonymous] 
//     [HttpPost("login")]
//     public async Task<IActionResult> Register(LoginRequest model, UserManager<IdentityUser> userManager, AuthService authservice)
//     {
//         var user = await userManager.FindByEmailAsync(model.Email);
//         if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
//         {
//             // Generate JWT token
//             var userDto = new User(
//                 Id: int.Parse(user.Id),
//                 Username: user.UserName,
//                 Email: user.Email,
//                 Roles: await userManager.GetRolesAsync(user),
//                 Password: "DAwda@Dawd@Dawd" // Replace with a secure key
//             );
//             var token = authservice.Create(userDto);
//
//             return (IActionResult)Results.Ok(token);
//             
//         }
//
//         return (IActionResult)Results.Unauthorized();    } 
// }