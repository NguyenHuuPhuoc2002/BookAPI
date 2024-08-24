using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface IChiTietHoaDonService
    {
        Task<IEnumerable<ChiTietHDModel>> GetAllAsync(Guid id);
    }
}
