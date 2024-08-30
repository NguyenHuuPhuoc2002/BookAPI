using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface ILoaiService
    {
        Task<IEnumerable<LoaiModel>> GetAllLoaiAsync();
        Task<LoaiModel> GetLoaiByIdAsync(string id);
        Task<bool> AddAsync(LoaiModel model);
        Task<bool> RemoveAsync(string id);
        Task<bool> UpdateAsync(LoaiModel model);
    }
}
