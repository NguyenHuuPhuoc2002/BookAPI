using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class LogInModel
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
