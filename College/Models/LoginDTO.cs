using System.ComponentModel.DataAnnotations;

namespace College.Models
{
    public class LoginDTO
    {
        [Required]
        public string Policy { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
