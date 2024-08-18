using AutoMapper;
using BookAPI.Data;
using BookAPI.Repositories.Database;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GioHangRepository> _logger;

        public GioHangRepository(DataContext context, IMapper mapper, ILogger<GioHangRepository> logger) 
        {
            _context = context;
            _mapper = mapper;
            _logger = logger; 
        }

        public async Task AddAsync(GioHang cart)
        {
            try
            {
                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thêm giỏ hàng của khách hàng {MaKH} vào csdl thành công", cart.MaKH);
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi thêm một giỏ hàng vào csdl");
                throw;
            }
        }

        public async Task<IEnumerable<GioHang>> GetAllCartsAsync()
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy tất cả giỏ hàng");
                var carts = await _context.gioHangs.ToListAsync();

                _logger.LogInformation("Trả về danh sách giỏ hàng thành công");
                return carts;
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi truy vấn lấy tất cả giỏ hàng");
                throw;
            }
        }

        public async Task<GioHang> GetCartByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy giỏ hàng với mã {id}", id);
                var cart = await _context.gioHangs.SingleOrDefaultAsync(p => p.GioHangId == id);

                if(cart == null )
                {
                    _logger.LogWarning("Không tìm thấy giỏ hàng có Id {id}", id);
                }

                _logger.LogInformation("Trả về một đối tượng giỏ hàng thành công");
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Xảy ra lỗi khi truy vấn lấy một đối tượng giỏ hàng");
                throw;
            }
        }

        public async Task<GioHang> GetCartByMaKhAsync(string maKh)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy giỏ hàng với mã KH {maKh}", maKh);
                var cart = await _context.gioHangs.SingleOrDefaultAsync(p => p.MaKH == maKh);

                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy giỏ hàng có mã KH {maKh}", maKh);
                }

                _logger.LogInformation("Trả về một đối tượng giỏ hàng thành công");
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi truy vấn lấy một đối tượng giỏ hàng");
                throw;
            }
        }
    }
}
