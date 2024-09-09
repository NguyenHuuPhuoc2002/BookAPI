using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "*"), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [MinLength(7, ErrorMessage = "Mật khẩu mới phải có ít nhất 7 ký tự.")]
        [MaxLength(20, ErrorMessage = "Mật khẩu mới không được vượt quá 15 ký tự.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "*")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }

    }

    public class ResetPasswordTokenModel
    {
        [Required(ErrorMessage = "*"), EmailAddress]
        public string Email { get; set; }

    }
}
