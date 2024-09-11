using Microsoft.AspNetCore.Identity;

namespace BookAPI.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<bool> AddRoleToUserAsync(string email, string roleName);

    }
}
