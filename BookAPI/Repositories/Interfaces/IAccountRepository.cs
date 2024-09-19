using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.RepositoryBase;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Repositories.Interfaces
{
    public interface IAccountRepository 
    {
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<IdentityUser> SignInAsync(SignInModel model);
        public Task<ApplicationUser> FindByEmailAsync(string email);
        public Task<ApplicationUser> FindByIdAsync(string id);
        public Task<IEnumerable<string>> GetRolesAsync(ApplicationUser model);
        public Task<bool> ChangePasswordAsync(ApplicationUser user, ChangePasswordModel model);
        Task<LinkMailModel> ForgetPassword(string email);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser model);
        Task<bool> ResetPasswordAsync(ApplicationUser model, string token, string newPassword);
    }
}
