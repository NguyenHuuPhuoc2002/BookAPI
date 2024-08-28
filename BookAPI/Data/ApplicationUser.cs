using Microsoft.AspNetCore.Identity;

namespace BookAPI.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Image { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
