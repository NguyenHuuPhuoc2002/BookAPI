using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _role;

        public RoleService(IRoleRepository role)
        {
            _role = role;
        }
        public async Task<bool> AddRoleAsync(string roleName)
        {
            return await _role.AddRoleAsync(roleName);
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            return await _role.DeleteRoleAsync(roleId);
        }

        public async Task<IEnumerable<IdentityRole>> GetRoleAllAsync()
        {
            return await _role.GetRoleAllAsync();
        }

        public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
        {
            return await _role.GetRoleByIdAsync(roleId);
        }

        public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
        {
            return await _role.GetRoleByNameAsync(roleName);
        }

        public async Task<bool> UpdateRoleAsync(IdentityRole role)
        {
            return await _role.UpdateRoleAsync(role);
        }
    }
}
