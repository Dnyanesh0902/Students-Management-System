using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Student_Management_API.Data;
using Student_Management_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Student_Management_API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
        }
        //Register User
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            //Pasword Hash
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
            return Ok("User Registered Successfully");
        }
        //login
        [HttpPost("login")]
        public async Task<IActionResult> Login(User loginUser)
        {
            var user = await _appDbContext.Users
                .FirstOrDefaultAsync(x => x.Username == loginUser.Username);

            if (user == null)
                return Unauthorized("Invalid Username");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);

            if (!isPasswordValid)
                return Unauthorized("Invalid Password");

            // Generate JWT Token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = jwtToken
            });
        }
    }
}
