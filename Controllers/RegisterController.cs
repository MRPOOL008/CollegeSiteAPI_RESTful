using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LoginRESTfulAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {

        private readonly string connectionString;

        public RegisterController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Register(string username, string password, string userRole)
        {
            try
            {

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //Chceck if user exists
                    string ChkUserQuery = $"SELECT * FROM Users WHERE UserEmail = '{username}'";

                    using (var command = new SqlCommand(ChkUserQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                ModelState.AddModelError("RegisterInfo", $"Email already exists. Login!");
                                return BadRequest(ModelState);
                            }
                        }
                    }


                    //insert to Users table
                    string query = $"INSERT INTO Users (UserEmail, Password, UserRole) VALUES ('{username}', '{password}', '{userRole}')";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        return Ok(new { Message = "Registration Successful" });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("RegisterInfo", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

    }
}
