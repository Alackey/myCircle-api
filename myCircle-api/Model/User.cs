using System.ComponentModel.DataAnnotations;

namespace myCircle_api.Model
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string PhotoUrl { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Second_email { get; set; }
    }
}