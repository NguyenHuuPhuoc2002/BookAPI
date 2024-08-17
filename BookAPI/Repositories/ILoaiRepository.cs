using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Repositories
{
    public interface ILoaiRepository
    {
        Task<IEnumerable<LoaiModel>> GetAllLoaiAsync();
        Task<LoaiModel> GetLoaiByIdAsync();
    }
}
