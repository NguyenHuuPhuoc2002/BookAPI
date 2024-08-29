using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
