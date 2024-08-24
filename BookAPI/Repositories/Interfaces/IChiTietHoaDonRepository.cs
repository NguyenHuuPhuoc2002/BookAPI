using BookAPI.Models;

namespace BookAPI.Repositories.Interfaces
{
    public interface IChiTietHoaDonRepository
    {
        Task<IEnumerable<ChiTietHDModel>> GetAllAsync(Guid id);
    }
}
