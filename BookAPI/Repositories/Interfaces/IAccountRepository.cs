using BookAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<IdentityUser> SignInAsync(SignInModel model);
    }
}
