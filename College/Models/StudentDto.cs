using System.ComponentModel.DataAnnotations;

namespace College.Models
{
    public class StudentDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Student name is required")]
        [StringLength(30)]
        public string StudentName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Address  is important")]
        public string Address { get; set; }
    }
}
