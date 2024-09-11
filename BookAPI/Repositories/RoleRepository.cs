using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
       
        public async Task<IEnumerable<IdentityRole>> GetRoleAllAsync()
        {
            try
            {
                var result = await _roleManager.Roles.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var result = await _roleManager.Roles.SingleOrDefaultAsync(p => p.Id == roleId);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
        {
            try
            {
                var result = await _roleManager.Roles.SingleOrDefaultAsync(p => p.Name.ToLower() == roleName.ToLower().Trim());
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AddRoleAsync(string roleName)
        {
            try
            {
                var role = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(role);
                return true;
               
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            try
            {
                var findRole = await _roleManager.FindByIdAsync(roleId);
                var result = await _roleManager.DeleteAsync(findRole);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateRoleAsync(IdentityRole role)
        {
            try
            {
                var findRole = await _roleManager.FindByIdAsync(role.Id);
                findRole.Name = role.Name;
                var result = await _roleManager.UpdateAsync(findRole);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
