using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Models;
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
                _logger.LogInformation("Thực hiện thêm sách {TenSach} vào giỏ hàng", cartItem.TenSach);
                var result = await _context.AddAsync(cartItemMap);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thêm sách {TenSach} vào giỏ hàng thành công", cartItem.TenSach);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Xảy ra lỗi khi thêm sách vào giỏ hàng");
                throw;
            }
        }

        public async Task ClearAllAsync(int id)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy tất cả sách với mã GioHangId {id}", id);
                var cartItem = _context.gioHangChiTiets.Where(p => p.GioHangId == id);
                _logger.LogInformation("Thực hiện xóa tất cả sách với mã GioHangId {id}", id);
                _context.gioHangChiTiets.RemoveRange(cartItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện xóa tất cả sách với mã GioHangId {id} thành công", id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi clear all sách trong giỏ");
                throw;
            }
        }

        public async Task DeleteAsync(CartModel cartItem)
        {
            try
            {
                if (cartItem != null)
                {
                    _logger.LogInformation("Truy vấn lấy sách trong giỏ với mã sách {id}", cartItem.MaSach);
                    var book = await _context.gioHangChiTiets.SingleOrDefaultAsync(p => p.MaSach == cartItem.MaSach &&
                                                                                        p.GioHangId == cartItem.GioHangId);
                    if (book == null)
                    {
                        _logger.LogWarning("Không tìm thấy sách trong giỏ với mã sách {id}", cartItem.MaSach);
                    }
                    else
                    {
                        _context.gioHangChiTiets.Remove(book);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Xóa sách trong giỏ thành công");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi xóa sách trong giỏ");
                throw;
            }
        }

        public async Task<IEnumerable<CartModel>> GetAllCartsAsync(int id)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy tất cả sách với Id {id} trong giỏ", id);
                var cart =  _context.gioHangChiTiets.Where(p => p.GioHangId == id);

                var result = await cart.Select(p => new CartModel
                {
                    Anh = p.Anh,
                    TenSach = p.TenSach,
                    MaSach = p.MaSach,
                    DonGia = p.DonGia,
                    SoLuong = p.SoLuong,
                    ThanhTien = p.ThanhTien,
                    GiamGia = p.GiamGia,
                    GioHangId = id
                }).ToListAsync();


                _logger.LogInformation("Trả về tất cả sách trong giỏ thành công {count}", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi truy vấn lấy một đối tượng sách");
                throw;
            }
        }

        public async Task<CartModel> GetCartItemByBookNameAsync(string maSach, int id)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy một sách với mã sách {maSach} trong giỏ", maSach);
                var cart = await _context.gioHangChiTiets.SingleOrDefaultAsync(p => p.GioHangId == id &&
                                                                                    p.MaSach == maSach);

                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với mã sách {maSach} trong giỏ", maSach);
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
                _logger.LogInformation("Truy vấn lấy sách với Id {id} trong giỏ", id);
                var cart = await _context.gioHangChiTiets.SingleOrDefaultAsync(p => p.GioHangId == id);

                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với Id {id} trong giỏ", id);
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

        public async Task UpdateAsync(int id, int amount)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy sách trong giỏ bằng với GioHangChiTietId {id}", id);
                var item = await _context.gioHangChiTiets.SingleOrDefaultAsync(p =>  p.GioHangChiTietId == id);
                
                if (item == null)
                {
                    _logger.LogWarning("Không tìm thấy sách trong giỏ với mã {id}", id);
                }
                else
                {
                    if(amount != 1)
                    {
                        item.SoLuong = amount;
                    }
                    else
                    {
                        item.SoLuong += 1;
                    }

                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cập nhật số lượng sách trong giỏ với mã KH {Id} thành công", id);
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
