using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _account;

        public AccountService(IAccountRepository account)
        {
            _account = account;
        }

        public async Task<bool> ChangePasswordAsync(ApplicationUser user, ChangePasswordModel model)
        {
            return await _account.ChangePasswordAsync(user, model);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _account.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            return await _account.FindByIdAsync(id);
        }

        public async Task<LinkMailModel> ForgetPassword(string email)
        {
            return await _account.ForgetPassword(email);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser model)
        {
            return await _account.GetRolesAsync(model);
        }

        public async Task<IdentityUser> SignInAsync(SignInModel model)
        {
            return await _account.SignInAsync(model);
        }

        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
        {
            return await _account.SignUpAsync(model);
        }
    }
}
