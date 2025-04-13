using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LoginRESTfulAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddAttendance : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public AddAttendance(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [Authorize(Roles = "Admin, Teacher")]
        [HttpPost]
        public IActionResult AddStudentAttendance(int studentID, int status, string date)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                //opening connection
                using(var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // string queryInsert = $"INSERT INTO StudentAttendance (StudentId, Status, Date) VALUES ({studentID}, {status}, '{date}')";

                    using(var command = new SqlCommand("CollegeSite_SP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@flag", "AddStudentAttendance");
                        command.Parameters.AddWithValue("@StudentID", studentID);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@date", date);
                        

                        command.ExecuteNonQuery();
                    }

                    return Ok(new { Message = "Attendance Marked for student" });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Add Attendance", $"An error occured: {ex.Message}");
                return BadRequest(ModelState);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddTeacherAttendance(int teacherID, int status, string date)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                //opening connection
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // string queryInsert = $"INSERT INTO TeacherAttendance (TeacherId, Status, Date) VALUES ({teacherID}, {status}, '{date}')";

                    using (var command = new SqlCommand("CollegeSite_SP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@flag", "AddEmployeeAttendance");
                        command.Parameters.AddWithValue("@teacherID", teacherID);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@date", date);

                        command.ExecuteNonQuery();
                    }

                    return Ok(new { Message = "Attendance Marked for Teacher" });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Add Attendance", $"An error occured: {ex.Message}");
                return BadRequest(ModelState);
            }
        }

    }
}
