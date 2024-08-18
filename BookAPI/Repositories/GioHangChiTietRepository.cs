using AutoMapper;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Database;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class GioHangChiTietRepository : IGioHangChiTietRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GioHangChiTietRepository> _logger;

        public GioHangChiTietRepository(DataContext context, IMapper mapper, ILogger<GioHangChiTietRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task AddAsync(CartModel cartItem)
        {
            try
            {
                var cartItemMap = _mapper.Map<GioHangChiTiet>(cartItem);
                _logger.LogInformation($"Thực hiện thêm sách {cartItem.TenSach} vào giỏ hàng");
                var result = await _context.AddAsync(cartItemMap);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Xảy ra lỗi khi thêm sách vào giỏ hàng");
                throw;
            }
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CartModel>> GetAllCartsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<CartModel> GetCartItemByBookNameAsync(string maSach, int id)
        {
            try
            {
                _logger.LogInformation($"Truy vấn lấy một sản phẩm với mã sách {maSach}");
                var cart = await _context.gioHangChiTiets.SingleOrDefaultAsync(p => p.GioHangId == id &&
                                                                                    p.MaSach == maSach);

                if (cart == null)
                {
                    _logger.LogWarning($"Không tìm thấy sách với mã {maSach}");
                }
                var result = _mapper.Map<CartModel>(cart);
                _logger.LogInformation("Trả về một đối tượng sách thành công");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Xảy ra lỗi khi truy vấn lấy một đối tượng sách");
                throw;
            }
        }

        public async Task<CartModel> GetCartItemByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Truy vấn lấy một sản phẩm với Id {id}");
                var cart = await _context.gioHangChiTiets.SingleOrDefaultAsync(p => p.GioHangId == id);

                if (cart == null)
                {
                    _logger.LogWarning($"Không tìm thấy sản phẩm với Id {id}");
                }
                var result = _mapper.Map<CartModel>(cart);
                _logger.LogInformation("Trả về một đối tượng sách thành công");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Xảy ra lỗi khi truy vấn lấy một đối tượng sách");
                throw;
            }
        }

        public async Task UpdateAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Truy vấn lấy sách trong giỏ bằng với mã {id}");
                var item = await _context.gioHangChiTiets.SingleOrDefaultAsync(p => p.GioHangChiTietId == id);

                if (item == null)
                {
                    _logger.LogWarning($"Không tìm thấy sách trong giỏ với mã {id}");
                }
                else
                {
                    item.SoLuong += 1;

                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                _logger.LogError("Xảy ra lỗi khi cập nhật số lượng");
                throw;
            }
        }
    }
}
