using LoginRESTfulAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;


namespace LoginRESTfulAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddTeacher : ControllerBase
    {

        private readonly IConfiguration _configuration;
        
        public AddTeacher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult InsertTeacher(string name, string Dob, string gender, string mobile, string email, string password)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                //opening connection
                using(var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //insert to table
                    // string queryInsert = $"INSERT INTO Teacher (TeacherName, DOB, Gender, Mobile, Email, Password) VALUES ('{name}', '{Dob}', '{gender}', '{mobile}', '{email}', '{password}')";

                    using (var cmd = new SqlCommand("CollegeSite_SP", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", "AddTeacher");
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@DOB", Dob);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Mobile", mobile);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        cmd.ExecuteNonQuery();
                    }

                    //select to fill grid view
                    // string querySelect = "SELECT * FROM Teacher";

                    using (var cmd = new SqlCommand("CollegeSite_SP", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", "GetAllTeacher");

                        List<TeacherModel> teacherList = new List<TeacherModel>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                teacherList.Add(new TeacherModel
                                {
                                    name = reader["TeacherName"].ToString(),
                                    Dob = reader["DOB"].ToString(),
                                    gender = reader["Gender"].ToString(),
                                    mobile = reader["Mobile"].ToString(),
                                    email = reader["Email"].ToString(),
                                    password = reader["Password"].ToString()

                                });

                            }
                        }

                        return Ok(teacherList);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddTeacher", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }
    }
}
