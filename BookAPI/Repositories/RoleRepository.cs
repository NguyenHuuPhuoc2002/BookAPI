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
                throw;
            }
        }
        public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var result = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
                if(result == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy role");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
        {
            try
            {
                var result = await _roleManager.Roles.SingleOrDefaultAsync(p => p.Name.ToLower() == roleName.ToLower().Trim());
                if (result == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy role");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        public async Task<bool> AddRoleAsync(string roleName)
        {
            try
            {
                var role = new IdentityRole(roleName);
                var findRole = await _roleManager.Roles.SingleOrDefaultAsync(p => p.Name.ToLower().Contains(roleName.ToLower().Trim()));
                if (findRole != null)
                {
                    throw new MissingFieldException("Role đã tồn tại");
                }
                var result = await _roleManager.CreateAsync(role);
                return true;
               
            }
            catch(Exception ex)
            {
                throw ;
            }
        }
        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            try
            {
                var findRole = await _roleManager.FindByIdAsync(roleId);
                if (findRole == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy role");
                }
                var result = await _roleManager.DeleteAsync(findRole);
                return true;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
        public async Task<bool> UpdateRoleAsync(IdentityRole role)
        {
            try
            {
                var findRole = await _roleManager.FindByIdAsync(role.Id);
                if (findRole == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy role");
                }
                findRole.Name = role.Name;
                var result = await _roleManager.UpdateAsync(findRole);
                return true;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
    }
}
