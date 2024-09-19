using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.RepositoryBase;

namespace BookAPI.Repositories.Interfaces
{
    public interface IChiTietHoaDonRepository
    {
        Task<IEnumerable<ChiTietHDModel>> GetAllAsync(Guid id);
    }
}
