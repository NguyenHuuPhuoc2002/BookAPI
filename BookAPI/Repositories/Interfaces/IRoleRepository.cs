﻿using Microsoft.AspNetCore.Identity;

namespace BookAPI.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<IdentityRole>> GetRoleAllAsync();
        Task<IdentityRole> GetRoleByNameAsync(string roleName);
        Task<IdentityRole> GetRoleByIdAsync(string roleId);
        Task<bool> AddRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleId);
        Task<bool> UpdateRoleAsync(IdentityRole role);
    }
}
