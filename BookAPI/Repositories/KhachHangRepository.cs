using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class KhachHangRepository : IKhachHangRepository
    {
        private readonly ILogger<KhachHangRepository> _logger;
        private readonly DataContext _context;

        public KhachHangRepository(DataContext context, ILogger<KhachHangRepository> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<KhachHang> CheckLogIn(LogInModel model)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy thông tin khách hàng với {UserName} và {Password}", model.UserName, model.Password);
                var khachHang = await _context.khachHangs.SingleOrDefaultAsync(p => p.MaKH == model.UserName &&
                                                                              p.MatKhau == model.Password);
                if (khachHang == null)
                {
                    _logger.LogWarning("Không tìm thấy khách hàng");
                }

                return khachHang;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,"Xảy ra lỗi khi lấy thông tin khách hàng");
                throw;
            }
        }
        public async Task<KhachHang> GetUserById(string maKH)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy thông tin khách hàng với {UserName}", maKH);
                var khachHang = await _context.khachHangs.SingleOrDefaultAsync(p => p.MaKH == maKH);
                if (khachHang == null)
                {
                    _logger.LogWarning("Không tìm thấy khách hàng");
                }

                return khachHang;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,"Xảy ra lỗi khi lấy thông tin khách hàng");
                throw;
            }
        }
    }
}
