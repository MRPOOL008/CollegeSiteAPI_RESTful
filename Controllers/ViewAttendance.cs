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
    public class ViewAttendance : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ViewAttendance(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize(Roles = "Admin, Teacher")]
        [HttpGet]
        public IActionResult GetStudentAttendance(string date)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                using(var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // string querySelect = $"SELECT * FROM StudentAttendance WHERE StudentId = {StudentId}";

                    using(var command = new SqlCommand("CollegeSite_SP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@flag", "GetStudentAttendance");
                        command.Parameters.AddWithValue("@date", date);

                        List<StudentAttendanceModel> studentAttendanceList = new List<StudentAttendanceModel>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                studentAttendanceList.Add(new StudentAttendanceModel()
                                {
                                    StudentName = reader["StudentName"].ToString(),
                                    Status = Convert.ToInt32(reader["Status"]),
                                    Date = reader["Date"].ToString()
                                }
                                );
                            }
                        }

                        return Ok(studentAttendanceList);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetStudentAttendance", $"An error occured: {ex}");
                return BadRequest(ModelState);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetTeacherAttendance(string date)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:SqlServerDb"];

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // string querySelect = $"SELECT * FROM TeacherAttendance WHERE TeacherId = {TeacherId}";

                    using (var command = new SqlCommand("CollegeSite_SP", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@flag", "GetEmployeeAttendance");
                        command.Parameters.AddWithValue("@date", date);

                        List<TeacherAttendanceModel> teacherAttendanceList = new List<TeacherAttendanceModel>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                teacherAttendanceList.Add(new TeacherAttendanceModel()
                                {
                                    TeacherName = reader["TeacherName"].ToString(),
                                    Status = Convert.ToInt32(reader["Status"]),
                                    Date = reader["Date"].ToString()
                                }
                                );
                            }
                        }

                        return Ok(teacherAttendanceList);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetStudentAttendance", $"An error occured: {ex}");
                return BadRequest(ModelState);
            }

        }

    }
}
