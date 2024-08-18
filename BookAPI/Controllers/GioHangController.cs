using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("giohang/sach")]
        public async Task<IActionResult> AddBook(string id)
        {
            try
            {
                var maKH = "phucduong";

                var cart = await _cart.GetCartByMaKhAsync(maKH) ?? await CreateCartAsync(maKH);

                var book = await _sach.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với mã {BookId}", id);
                    return NotFound();
                }

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
                    await _cartItem.UpdateAsync(cart.GioHangId);
                    _logger.LogInformation("Cập nhật số lượng sách cho khách hàng với mã {CustomerId} thành công", maKH);
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi thêm sách vào giỏ hàng");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        private async Task<GioHang> CreateCartAsync(string maKH)
        {
            var createCart = new GioHang { MaKH = maKH };
            await _cart.AddAsync(createCart);
            _logger.LogInformation("Tạo giỏ hàng cho khách hàng có mã {CustomerId}", maKH);
            return createCart;
        }
    }
}
