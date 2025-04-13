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
    public class AddClass : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public AddClass(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult InsertClass(string className)
        {
            try
            {
                //get connection string
                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                //open connection
                using (var connection = new SqlConnection(connectionString))
                {

                    connection.Open();

                    // string queryInsert = $"INSERT INTO Class (ClassName) VALUES ('{className}')";

                    //insert to table class
                    using (var cmd = new SqlCommand("CollegeSite_SP", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", "AddClass");
                        cmd.Parameters.AddWithValue("@ClassName", className);

                        cmd.ExecuteNonQuery();

                    }

                    //select from the table to fill the grid view
                    //string querySelect = "SELECT * FROM Class";
                    using (var cmd = new SqlCommand("CollegeSite_SP", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@flag", "GetAllClass");

                        List<ClassModel> classList = new List<ClassModel>();

                        //use reader and add all rows to a list
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                classList.Add(new ClassModel
                                {
                                    ClassId = reader.GetInt32(0),    // Read first column (ClassId)
                                    ClassName = reader.GetString(1)  // Read second column (ClassName)
                                });
                            }
                        }

                        return Ok(classList);

                    }

                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddClass", $"An error occurred: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

    }
}
