using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.RepositoryBase;

namespace BookAPI.Repositories.Interfaces
{
    public interface ILoaiRepository
    {
        Task<IEnumerable<LoaiModel>> GetAllLoaiAsync();
        Task<LoaiModel> GetLoaiByIdAsync(string id);
        Task<bool> AddAsync(LoaiModel model);
        Task<bool> RemoveAsync(string id);
        Task<bool> UpdateAsync(LoaiModel model);
    }
}
