using BookAPI.Data;
using BookAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<IdentityUser> SignInAsync(SignInModel model);
        public Task<ApplicationUser> FindByEmailAsync(string email);
        public Task<bool> ChangePasswordAsync(ApplicationUser user, ChangePasswordModel model);
        public Task<ApplicationUser> FindByIdAsync(string id);
        public Task<IEnumerable<string>> GetRolesAsync(ApplicationUser model);
        Task<LinkMailModel> ForgetPassword(string email);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser model);
        Task<bool> ResetPasswordAsync(ApplicationUser model, string token, string newPassword);
    }
}
