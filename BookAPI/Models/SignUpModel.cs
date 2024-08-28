using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class SignUpModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; } 
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public int? Gender{ get; set; }
        public IFormFile? Image { get; set; }
        public string? Hinh{ get; set; }

    }
}
