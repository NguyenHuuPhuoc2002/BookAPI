using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface ISachService
    {
        Task<IEnumerable<SachModel>> GetAllBooksAsync(string? maLoai, int page, int pageSize);
        Task<SachModel> GetBookByIdAsync(string id);
        Task<IEnumerable<SachModel>> SearchBookAsync(string key, int page, int pageSize);
        Task<IEnumerable<SachModel>> SearchBookByNXBAsync(string key, int page, int pageSize);
        Task UpdateInventoryQuantity(Dictionary<string, int> books);

        Task<bool> AddAsync(Sach model);
        Task<bool> UpdateAsync(Sach model);
        Task<bool> DeleteAsync(string id);
    }
}
