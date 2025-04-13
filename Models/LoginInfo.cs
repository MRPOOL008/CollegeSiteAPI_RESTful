using System.ComponentModel.DataAnnotations;

namespace LoginRESTfulAPI.Models
{
    public class LoginInfo
    {

        public string userEmail { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;

        public string userRole { get; set; } = string.Empty;


    }
}
