using BookAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<bool> AddRoleToUserAsync(string email, string roleName);
    }
}
