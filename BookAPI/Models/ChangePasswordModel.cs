using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [MinLength(7, ErrorMessage = "Mật khẩu mới phải có ít nhất 7 ký tự.")]
        [MaxLength(20, ErrorMessage = "Mật khẩu mới không được vượt quá 15 ký tự.")]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
