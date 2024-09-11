using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
        {
              _userManager = userManager;
              _roleManager = roleManager; 
        }
        public async Task<bool> AddRoleToUserAsync(string email, string roleName)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return false;
                }
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    return false;
                }
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
    }
}
