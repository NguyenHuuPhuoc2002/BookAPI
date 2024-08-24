using BookAPI.Database;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class ChiTietHoaDonRepository : IChiTietHoaDonRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<ChiTietHoaDonRepository> _logger;

        public ChiTietHoaDonRepository(DataContext context, ILogger<ChiTietHoaDonRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<ChiTietHDModel>> GetAllAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Thực hiện truy vấn lấy chi tiết hóa đơn");
                var data = from ct in _context.ChiTietHoaDons
                           join s in _context.Sachs on ct.MaSach equals s.MaSach
                           where ct.MaHD == id
                           select new ChiTietHDModel
                           {
                               MaCT = ct.MaCT,
                               MaHD = ct.MaHD,
                               MaSach = s.MaSach,
                               DonGia = ct.DonGia,
                               SoLuong = ct.SoLuong,
                               GiamGia = ct.GiamGia,
                               Anh = s.Anh,
                           };
                _logger.LogInformation("Thực hiện truy vấn lấy chi tiết hóa đơn thành công");
                return await data.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,"Xảy ra lỗi khi thực hiện truy vấn lấy chi tiết hóa đơn");
                throw;
            }
        }
    }
}
