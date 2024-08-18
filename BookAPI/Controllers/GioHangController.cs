using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangRepository _cart;
        private readonly IGioHangChiTietRepository _cartItem;
        private readonly ILogger<GioHangController> _logger;
        private readonly ISachRepository _sach;

        public GioHangController(IGioHangRepository cart, IGioHangChiTietRepository cartItem,
                                ILogger<GioHangController> logger, ISachRepository sach)
        {
            _cart = cart;
            _cartItem = cartItem;
            _logger = logger;
            _sach = sach;
        }

        [HttpGet("giohang")]
        public async Task<IActionResult> GetCart()
        {
            var maKh = "phucduong";
            _logger.LogInformation("Yêu cầu lấy giỏ hàng với mã KH {id}", maKh);
            var cart = await _cart.GetCartByMaKhAsync(maKh);
            _logger.LogInformation("Yêu cầu lấy tất cả sách trong giỏ hàng của khách hàng với mã {id} và mã giỏ hàng {mgh}", maKh, cart.GioHangId);
            var cartItems = await _cartItem.GetAllCartsAsync(cart.GioHangId);

            _logger.LogInformation("Yêu cầu lấy giỏ hàng thành công {count}", cartItems.Count());
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lấy thành công!",
                Data = cartItems

            });
        }

        [HttpPost("giohang/add")]
        public async Task<IActionResult> AddBook(string id)
        {
            try
            {
                var maKH = "phucduong";
                _logger.LogInformation("Nhận yêu cầu lấy giỏ hàng với mã KH {MaKH}", maKH);
                var cart = await _cart.GetCartByMaKhAsync(maKH) ?? await CreateCartAsync(maKH);
                _logger.LogInformation("Nhận yêu cầu lấy sách với mã sách {MaSach}", id);
                var book = await _sach.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với mã {BookId}", id);
                    return NotFound();
                }
                _logger.LogInformation("Nhận yêu cầu lấy sách trong giỏ hàng với mã {id} và giỏ hàng ID {gioHangId}", id, cart.GioHangId);
                var cartItem = await _cartItem.GetCartItemByBookNameAsync(id, cart.GioHangId);
                if (cartItem == null)
                {
                    var result = new CartModel
                    {
                        Anh = book.Anh ?? "",
                        TenSach = book.TenSach,
                        DonGia = book.Gia ?? 0,
                        SoLuong = 1,
                        ThanhTien = book.SoLuong * book.Gia ?? 0,
                        GiamGia = 0,
                        GioHangId = cart.GioHangId,
                        MaSach = book.MaSach
                    };

                    await _cartItem.AddAsync(result);
                    _logger.LogInformation("Thêm sách với mã {BookId} vào giỏ hàng cho khách hàng với mã {CustomerId} thành công", result.MaSach, maKH);
                    return Ok(result);
                }
                else
                {
                    await _cartItem.UpdateAsync(cartItem.GioHangChiTietId, 1);
                    _logger.LogInformation("Cập nhật số lượng sách cho khách hàng với mã {CustomerId} thành công", maKH);
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi thêm sách vào giỏ hàng");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        private async Task<GioHang> CreateCartAsync(string maKH)
        {
            try
            {
                var createCart = new GioHang { MaKH = maKH };
                await _cart.AddAsync(createCart);
                _logger.LogInformation("Tạo giỏ hàng cho khách hàng có mã {CustomerId}", maKH);
                return createCart;
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi thêm giỏ hàng");
                throw;
            }
        }

        [HttpDelete("giohang/delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var khachHang = "phucduong";
            if(!string.IsNullOrEmpty(id))
            {
                _logger.LogInformation("Xóa sách với mã {MaSach} từ giỏ hàng của khách hàng {CustomerId}", id, khachHang);
                var cart = await _cart.GetCartByMaKhAsync(khachHang);
                var book = await _sach.GetBookByIdAsync(id);

                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với mã {BookId}", id);
                    return NotFound();
                }
                var cartItem = await _cartItem.GetCartItemByBookNameAsync(id, cart.GioHangId);

                if(cartItem != null)
                {
                    await _cartItem.DeleteAsync(cartItem);
                    _logger.LogWarning("Xóa thành công {id}", cartItem.MaSach);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Xóa thành công!",
                        Data = cartItem
                    });
                }
            }
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Xóa không thành công!"
            });
        }

        [HttpPut("giohang/update-amount")]
        public async Task<IActionResult> UpdateAmount(string id, int amount) 
        {
            if(amount <= 0)
            {
                _logger.LogWarning("Số lượng nhập vào không hợp lệ");
                return BadRequest();
            }
            var maKH = "phucduong";

            _logger.LogInformation("Yêu cầu cập nhật số lượng sách. Mã KH: {MaKH}, Mã sách: {MaSach}, Số lượng: {Amount}", maKH, id, amount);
            var cart = await _cart.GetCartByMaKhAsync(maKH) ;// lấy giỏ hàng theo mã khách hàng
            var book = await _sach.GetBookByIdAsync(id); // lấy sách theo max sách

            if (book == null)
            {
                _logger.LogWarning("Không tìm thấy sách với mã {BookId}", id);
                return NotFound();
            }
            
            //Lấy một sách trong giỏ hàng chi tiết bằng mã sách và mã giỏ hàng
            var cartItem = await _cartItem.GetCartItemByBookNameAsync(id, cart.GioHangId);
            if (cartItem == null)
            {
                _logger.LogWarning("Không tìm thấy sách trong giỏ hàng với GioHangId {GioHangId}", cart.GioHangId);
                return NotFound();
            }
            //Cập nhật
            await _cartItem.UpdateAsync(cartItem.GioHangChiTietId, amount);
            _logger.LogInformation("Cập nhật số lượng thành công");
            return NoContent();
        }
    }
}
