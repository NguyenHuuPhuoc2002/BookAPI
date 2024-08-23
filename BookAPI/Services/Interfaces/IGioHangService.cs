using BookAPI.Data;

namespace BookAPI.Services.Interfaces
{
    public interface IGioHangService
    {
        Task<IEnumerable<GioHang>> GetAllCartsAsync();
        Task<GioHang> GetCartByIdAsync(int id);
        Task<GioHang> GetCartByMaKhAsync(string maKh);
        Task AddRangeChiTietHdAsync(IEnumerable<ChiTietHoaDon> cthds);
        Task AddAsync(GioHang cart);
        Task AddHoaDonAsync(HoaDon model);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
