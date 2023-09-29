using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class TokenController : ControllerBase {
    private readonly IConfiguration _config;
    private readonly DataContext _context;

    public TokenController(IConfiguration config, DataContext context) {
        _config = config;
        _context = context;
    }

    public async Task<IActionResult> Post(LoginModel loginModel) {
        if (loginModel is not { Login: not null, Password: not null }) return BadRequest();

        var user = await GetFullUserInfoAsync(loginModel.Login, loginModel.Password);

        if (user is null) return BadRequest("Invalid credentials.");

        var role = await GetUserRoleAsync(user);

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new("UserId", user.Id.ToString()),
            new("Login", user.Login!)
        };
        if (role != null) claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name!));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: signIn
        );

        return Ok(new TokenControllerPostResponse {
            JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
            Login = user.Login,
            Role = role
        });
    }

    private async Task<User?> GetFullUserInfoAsync(string login, string password) {
        return await _context.Users?.FirstOrDefaultAsync(u => u.Login == login && u.Password == password)!;
    }

    private async Task<Role?> GetUserRoleAsync(User user) {
        var userRole = await _context.UserRoles!.Include(ur => ur.Role).FirstOrDefaultAsync(ur => ur.UserId == user.Id);

        return userRole?.Role;
    }
}