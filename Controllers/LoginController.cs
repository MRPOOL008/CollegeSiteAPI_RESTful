using LoginRESTfulAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LoginRESTfulAPI.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginRESTfulAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //private readonly string connectionString;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {

            // connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
            _configuration = configuration;

        }

        //[Authorize]
        [HttpGet]
        public IActionResult Login(string userEmail, string password)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT * FROM Users WHERE UserEmail = '{userEmail}' AND Password = '{password}'";

                    using (var command = new SqlCommand(query, connection))
                    {

                        LoginInfo userInfo = new LoginInfo();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) 
                            {
                                reader.Read();
                                string userRole = reader["UserRole"].ToString();
                                var token = GenerateJwtTokens(userEmail, userRole);
                                return Ok(new { Token = token, UserRole = userRole, Message = "Login successful!" });
                                //return Ok( new { Message = "Login Success!"});
                            }
                            else
                            {
                                ModelState.AddModelError("LoginInfo", "Invalid email or password.");
                                return BadRequest(ModelState);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                //Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError("LoginInfo", "An error occurred during login.");
                return BadRequest(ModelState);

            }
        }

        [HttpPost]
        public string GenerateJwtTokens(string userEmail, string userRole)
        {

            //get Jwt key
            var JwtKey = _configuration["JwtSettings:Key"];


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userRole)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
