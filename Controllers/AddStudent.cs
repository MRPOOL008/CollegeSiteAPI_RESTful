using LoginRESTfulAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace LoginRESTfulAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddStudent : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public AddStudent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult InsertStudent(string name, string Dob, string gender, string mobile, string email, string password)
        {
            try
            {

                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                //opening connection
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //insert to table
                    // string queryInsert = $"INSERT INTO Student (StudentName, DOB, Gender, Mobile, Email, Password) VALUES ('{name}', '{Dob}', '{gender}', '{mobile}', '{email}', '{password}')";

                    using (var cmd = new SqlCommand("CollegeSite_SP", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", "AddStudent");
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@DOB", Dob);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Mobile", mobile);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        cmd.ExecuteNonQuery();
                    }

                    //select to fill grid view
                    // string querySelect = "SELECT * FROM Student";

                    using (var cmd = new SqlCommand("CollegeSite_SP", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", "GetAllStudent");

                        List<StudentModel> studentList = new List<StudentModel>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentList.Add(new StudentModel
                                {
                                    name = reader["StudentName"].ToString(),
                                    Dob = reader["DOB"].ToString(),
                                    gender = reader["Gender"].ToString(),
                                    mobile = reader["Mobile"].ToString(),
                                    email = reader["Email"].ToString(),
                                    password = reader["Password"].ToString()

                                });

                            }
                        }

                        return Ok(studentList);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddStudent", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

    }
}
