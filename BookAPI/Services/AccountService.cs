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
