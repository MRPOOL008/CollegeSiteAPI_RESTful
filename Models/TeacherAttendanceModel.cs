namespace LoginRESTfulAPI.Models
{
    public class TeacherAttendanceModel
    {
        public string TeacherName { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Date { get; set; } = string.Empty;
    }
}
