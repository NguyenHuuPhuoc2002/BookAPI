using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Repositories.Interfaces
{
    public interface ILoaiRepository
    {
        Task<IEnumerable<LoaiModel>> GetAllLoaiAsync();
        Task<LoaiModel> GetLoaiByIdAsync(string id);
    }
}
