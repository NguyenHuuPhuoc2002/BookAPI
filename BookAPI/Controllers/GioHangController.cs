using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using AutoMapper;
using EcommerceWeb.ViewModels;
using EcommerceWeb.Services;
using Azure;
using System.Security.Claims;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _cart;
        private readonly IGioHangChiTietService _cartItem;
        private readonly ILogger<GioHangController> _logger;
        private readonly ISachService _sach;
        private readonly IMapper _mapper;
        private readonly IVnPayService _vnPayService;
        
        private static CheckoutModel _model { get; set; }
        public GioHangController(IGioHangService cart, IGioHangChiTietService cartItem, IVnPayService vnPayService,
                                ILogger<GioHangController> logger, ISachService sach, IMapper mapper)
        {
           // maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _cart = cart;
            _cartItem = cartItem;
            _logger = logger;
            _sach = sach;
            _mapper = mapper;
            _vnPayService = vnPayService;
        }

        [HttpGet("carts")]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            try
            {
                var cart = await _cart.GetCartByMaKhAsync(maKh) ?? await CreateCartAsync(maKh);
                var cartItems = await _cartItem.GetAllCartsAsync(cart.GioHangId);

                _logger.LogInformation("Yêu cầu lấy tất cả sách trong giỏ hàng với {MaKh} thành công {count}", maKh, cartItems.Count());
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy thành công!",
                    Data = cartItems

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Đã xảy ra lỗi khi lấy giỏ hàng");

                // Trả về mã trạng thái 500 cùng với thông báo lỗi từ ngoại lệ
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddBook(string id)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Đầu vào không hợp lệ");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ"
                });
            }

            try
            {
                _logger.LogInformation("Nhận yêu cầu lấy giỏ hàng với mã KH {MaKH}", maKh);
                var cart = await _cart.GetCartByMaKhAsync(maKh) ?? await CreateCartAsync(maKh);
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
                        ThanhTien = book.Gia ?? 0,
                        GiamGia = 0,
                        GioHangId = cart.GioHangId,
                        MaSach = book.MaSach
                    };

                    await _cartItem.AddAsync(result);
                    _logger.LogInformation("Thêm sách với mã {BookId} vào giỏ hàng cho khách hàng với mã {CustomerId} thành công", result.MaSach, maKh);
                    return Ok(result);
                }
                else
                {
                    await _cartItem.UpdateAsync(cartItem.GioHangChiTietId, 1);
                    _logger.LogInformation("Cập nhật số lượng sách cho khách hàng với mã {CustomerId} thành công", maKh);
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi thêm sách vào giỏ hàng");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Đầu vào không hợp lệ");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ"
                });
            }
            try
            {
                _logger.LogInformation("Xóa sách với mã {MaSach} từ giỏ hàng của khách hàng {CustomerId}", id, maKh);

                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy giỏ hàng của khách hàng {CustomerId}", maKh);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy giỏ hàng của khách hàng."
                    });
                }

                var book = await _sach.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với mã {BookId}", id);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Không tìm thấy sách với mã {id}."
                    });
                }

                var cartItem = await _cartItem.GetCartItemByBookNameAsync(id, cart.GioHangId);
                if (cartItem != null)
                {
                    await _cartItem.DeleteAsync(cartItem);
                    _logger.LogInformation("Xóa thành công sách với mã {BookId} từ giỏ hàng của khách hàng {CustomerId}", id, maKh);
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning("Không tìm thấy sách trong giỏ hàng với mã {BookId}", id);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Không tìm thấy sách với mã {id} trong giỏ hàng."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi thực hiện xóa sách với mã {BookId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }


        [HttpPut("update-amount")]
        [Authorize]
        public async Task<IActionResult> UpdateAmount(string id, int amount)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(id) || amount <= 0)
            {
                _logger.LogWarning("Đầu vào không hợp lệ");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ"
                });
            }
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật số lượng sách. Mã KH: {MaKH}, Mã sách: {MaSach}, Số lượng: {Amount}", maKh, id, amount);
                var cart = await _cart.GetCartByMaKhAsync(maKh);
                if (cart == null)
                {
                    _logger.LogWarning("Không tìm thấy giỏ hàng cho khách hàng với mã KH {MaKH}", maKh);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy giỏ hàng cho khách hàng."
                    });
                }

                var book = await _sach.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách với mã {BookId}", id);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Không tìm thấy sách với mã {id}."
                    });
                }

                var cartItem = await _cartItem.GetCartItemByBookNameAsync(id, cart.GioHangId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Không tìm thấy sách trong giỏ hàng với GioHangId {GioHangId}", cart.GioHangId);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Sách không có trong giỏ hàng."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi thực hiện cập nhật số lượng sách. Mã sách: {BookId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi thực hiện cập nhật số lượng sách. Vui lòng thử lại sau."
                });
            }
        }

        [HttpDelete("clear-all")]
        [Authorize]
        public async Task<IActionResult> ClearAll()
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Yêu cầu xóa tất cả sách trong giỏ với mã KH {id}", maKh);
            try
            {
                var cart = await _cart.GetCartByMaKhAsync(maKh);
                await _cartItem.ClearAllAsync(cart.GioHangId);
                _logger.LogInformation("Yêu cầu xóa tất cả sách trong giỏ với mã KH {id} thành công", maKh);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Xóa tất cả sách thành công!",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi clear giỏ hàng");
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
    }
}