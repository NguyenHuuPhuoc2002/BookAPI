using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface IHoaDonService
    {
        Task<IEnumerable<HoaDonModel>> GetOrdersByMaKhAsync(string maKh, int page, int pageSize);
        Task<HoaDonModel> GetOrderByIdAsync(Guid id, string maKh);
    }
}
