using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetAuth.Controllers;
using DotNetAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAuth;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(IConfiguration configuration, UserManager<IdentityUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    
    public async Task<string> CreateJwt(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        
        var privateKey = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"] ?? string.Empty);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(privateKey),
            SecurityAlgorithms.HmacSha256);

        var jwtClaims = await GenerateClaims(user);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Subject =  jwtClaims,
            Audience = _configuration["JwtSettings:ValidAudiences"],
            Issuer = _configuration["JwtSettings:ValidIssuer"],
        };
        
        
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
    
    
    private async Task<ClaimsIdentity> GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();

        // Required for identity mapping
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        
        // Metadata about logged in user
        ci.AddClaim(new Claim(ClaimTypes.Name, user.Username));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        var identityUser = _userManager.FindByEmailAsync(user.Email).Result;
        if (identityUser != null)
        {
            ci.AddClaim(new Claim("two_fa_enabled", identityUser.TwoFactorEnabled.ToString()));
        }
        
        // Add each role
        foreach (var role in user.Roles)
            ci.AddClaim(new Claim(ClaimTypes.Role, role));
    
        return ci;
    }
}