using BookAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<IdentityUser> SignInAsync(SignInModel model);
    }
}
