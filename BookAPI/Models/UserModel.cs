using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "*")]
        public string Email { get; set; }
        [Required(ErrorMessage = "*")]
        public string Password { get; set; }
        [Required(ErrorMessage = "*")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "*")]
        public string LastName { get; set; }
        public int Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Image {  get; set; }
        [Required(ErrorMessage = "*")]
        public string PhoneNumber { get; set; }

    }
    public class RoleModel
    {
        public Guid UserID { get; set; }
        public Guid RoleID { get; set; }

    }
}
