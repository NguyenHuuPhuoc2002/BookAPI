using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _user;

        public UserService(IUserRepository user) 
        {
            _user = user;
        }
        public async Task<bool> AddAsync(UserModel model)
        {
            return await _user.AddAsync(model);
        }

        public async Task<bool> DeleteAsync(string email)
        {
            return await _user.DeleteAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(int page, int pageSize)
        {
            return await _user.GetAllAsync(page, pageSize);
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _user.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> Search(string key, int page, int pageSize)
        {
            return await _user.Search(key, page, pageSize);
        }

        public async Task<bool> UpdateAsync(ApplicationUser model)
        {
            return await _user.UpdateAsync(model);
        }
    }
}
