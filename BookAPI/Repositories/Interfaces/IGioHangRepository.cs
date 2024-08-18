using BookAPI.Data;

namespace BookAPI.Repositories.Interfaces
{
    public interface IGioHangRepository
    {
        Task<IEnumerable<GioHang>> GetAllCartsAsync();
        Task<GioHang> GetCartByIdAsync(int id);
        Task<GioHang> GetCartByMaKhAsync(string maKh);
        Task AddAsync(GioHang cart);
    }
}
