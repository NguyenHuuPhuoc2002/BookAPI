using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        public UserRoleRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
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

        public async Task<bool> DeleteRoleUserAsync(string email, string roleName)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var isInRole = await _userManager.IsInRoleAsync(user, roleName);
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
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

        public async Task<IEnumerable<UserRoleModel>> GetAllAsync(string email)
        {
            try
            {
                var result = from ur in _context.UserRoles
                             join u in _context.Users on ur.UserId equals u.Id
                             join r in _context.Roles on ur.RoleId equals r.Id
                             // Điều kiện lọc nếu email không null, nếu null thì lấy tất cả
                             where string.IsNullOrEmpty(email) || u.Email == email
                             select new UserRoleModel
                             {
                                 User = u.UserName, 
                                 Role = r.Name      
                             };
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
