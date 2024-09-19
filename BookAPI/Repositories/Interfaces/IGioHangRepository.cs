using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.RepositoryBase;

namespace BookAPI.Repositories.Interfaces
{
    public interface IGioHangRepository 
    {
        Task<IEnumerable<GioHang>> GetAllCartsAsync();
        Task<GioHang> GetCartByIdAsync(int id);
        Task<GioHang> GetCartByMaKhAsync(string maKh);
        Task AddAsync(GioHang cart);
        Task AddHoaDonAsync(HoaDon model);
        Task AddRangeChiTietHdAsync(IEnumerable<ChiTietHoaDon> cthds);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
