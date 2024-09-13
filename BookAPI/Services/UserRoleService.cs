using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _role;

        public UserRoleService(IUserRoleRepository role) 
        {
            _role = role;
        }

        public async Task<bool> AddRoleToUserAsync(string email, string roleName)
        {
            return await _role.AddRoleToUserAsync(email, roleName);
        }

        public async Task<bool> DeleteRoleUserAsync(string email, string roleName)
        {
            return await _role.DeleteRoleUserAsync(email,roleName);
        }

        public async Task<IEnumerable<UserRoleModel>> GetAllAsync(string email)
        {
            return await _role.GetAllAsync(email);
        }
    }
}
