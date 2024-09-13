using BookAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleModel>> GetAllAsync(string email);
        Task<bool> AddRoleToUserAsync(string email, string roleName);
        Task<bool> DeleteRoleUserAsync(string email, string roleName);
    }
}
