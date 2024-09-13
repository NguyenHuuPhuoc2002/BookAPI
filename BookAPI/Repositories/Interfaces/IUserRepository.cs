using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync(int page, int pageSize);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> Search(string key, int page, int pageSize);
        Task<bool> AddAsync(UserModel model);
        Task<bool> UpdateAsync(ApplicationUser model);
        Task<bool> DeleteAsync(string email);
    }
}
