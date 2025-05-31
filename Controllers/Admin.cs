
using BankingAPI.Models;
using BankingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _configuration;

        public AdminController(IAdminRepository adminRepository, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] AdminLoginRequest request)
        {
            // Validate admin credentials using repository
            var admin = _adminRepository.ValidateAdmin(request.Username, request.Password);
            if (admin == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate JWT token
            var token = GenerateJwtToken(admin);

            return Ok(new
            {
                Message = "Login successful!",
                Token = token
            });
        }

        private string GenerateJwtToken(Admin admin)
        {
            var key = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("JwtSettings:SecretKey is missing in the configuration.");
            }

            var issuer = _configuration["JwtSettings:ValidIssuer"];
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentNullException("JwtSettings:ValidIssuer is missing in the configuration.");
            }

            var audience = _configuration["JwtSettings:ValidAudience"];
            if (string.IsNullOrEmpty(audience))
            {
                throw new ArgumentNullException("JwtSettings:ValidAudience is missing in the configuration.");
            }

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, admin.Username),
        new Claim(ClaimTypes.Role, "Admin")
    };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}