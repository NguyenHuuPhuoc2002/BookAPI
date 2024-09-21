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
                var result = await _context.ChiTietHoaDons.Include(p => p.Sach).Where(p => p.MaHD == id).ToListAsync();
                var data = result.Select(p => new ChiTietHDModel
                {
                    MaCT = p.MaCT,
                    MaHD = p.MaHD,
                    MaSach = p.Sach.MaSach,
                    DonGia = p.DonGia,
                    SoLuong = p.SoLuong,
                    GiamGia = p.GiamGia,
                    Anh = p.Sach.Anh
                }).ToList();
                _logger.LogInformation("Thực hiện truy vấn lấy chi tiết hóa đơn thành công");
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,"Xảy ra lỗi khi thực hiện truy vấn lấy chi tiết hóa đơn");
                throw;
            }
        }
    }
}
