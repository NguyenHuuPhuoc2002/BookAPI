using BookAPI.Models;

namespace BookAPI.Repositories
{
    public interface ISachRepository
    {
        Task<IEnumerable<SachModel>> GetAllBooksAsync(string? maLoai, int page, int pageSize);
        Task<SachModel> GetBookByIdAsync(string id);
        Task<IEnumerable<SachModel>> SearchBookAsync(string key, int page, int pageSize);

    }
}
