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
using Common.Exceptions;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly IGioHangService _cart;
        private readonly IGioHangChiTietService _cartItem;
        private readonly ILogger<CartsController> _logger;
        private readonly ISachService _sach;
        private readonly IMapper _mapper;
        private readonly IVnPayService _vnPayService;

        private static CheckoutModel _model { get; set; }
        public CartsController(IGioHangService cart, IGioHangChiTietService cartItem, IVnPayService vnPayService,
                                ILogger<CartsController> logger, ISachService sach, IMapper mapper)
        {
            _cart = cart;
            _cartItem = cartItem;
            _logger = logger;
            _sach = sach;
            _mapper = mapper;
            _vnPayService = vnPayService;
        }
        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse { Success = false, Message = "Không tìm thấy email!" });
            }
            var cart = await _cart.GetCartByMaKhAsync(email);
            var cartItems = await _cartItem.GetAllCartsAsync(cart.GioHangId);
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null)
            {
                throw new KeyNotFoundException();
            }
            else if (response.VnPayResponseCode != "00")
            {
                throw new AppException("Thanh toán không thành công");
            }
            #region Tạo Đơn Hàng

            var hoaDon = new HoaDon
            {
                MaKH = email,
                HoTen = _model.HoTen,
                DiaChi = _model.DiaChi,
                DienThoai = _model.SoDienThoai,
                NgayDat = DateTime.Now,
                NgayGiao = DateTime.Now.AddDays(3),
                CachThanhToan = MyConstants.PAYMENT_VNPAY,
                CachVanChuyen = MyConstants.SHIPPING_WAY,
                PhiVanChuyen = MyConstants.SHIPPING_FEE,
                MaTrangThai = MyConstants.STATE_NEW_ORDER,
                GhiChu = _model.GhiChu,
                TongTien = cartItems.Sum(p => p.SoLuong * p.DonGia)
            };

            await _cart.BeginTransactionAsync();
            try
            {
                await _cart.CommitTransactionAsync();
                // Thêm hóa đơn
                await _cart.AddHoaDonAsync(hoaDon);
                _logger.LogInformation("Thêm đơn hàng thành công");

                // Thêm chi tiết đơn hàng
                _logger.LogInformation("Thêm chi tiết đơn hàng");
                var cthds = new List<ChiTietHoaDon>();
                foreach (var item in cartItems)
                {
                    cthds.Add(new ChiTietHoaDon
                    {
                        MaHD = hoaDon.MaHD,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        MaSach = item.MaSach,
                        GiamGia = 0
                    });
                }
                await _cart.AddRangeChiTietHdAsync(cthds);

                // Cập nhật số lượng tồn kho
                var dictionary = new Dictionary<string, int>();
                foreach (var item in cartItems)
                {
                    dictionary[item.MaSach] = item.SoLuong;
                }
                await _sach.UpdateInventoryQuantity(dictionary);
                _logger.LogInformation("Thêm chi tiết đơn hàng thành công");
                // Xóa các mặt hàng trong giỏ hàng của khách hàng
                // Commit transaction sau khi tất cả các thao tác đã hoàn thành
               
                await _cartItem.ClearAllAsync(cart.GioHangId);
                _logger.LogInformation("Xóa mặt hàng sau khi thanh toán của khách hàng {maKh} thành công", email);
                _logger.LogInformation("Thanh toán cho khách hàng {maKh} thành công", email);
                // Trả về kết quả
                var order = _mapper.Map<HoaDonModel>(hoaDon);
               
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thanh toán thành công",
                    Data = order
                });
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi xảy ra
                await _cart.RollbackTransactionAsync();
                _logger.LogError("Xảy ra lỗi khi thanh toán đơn hàng cho khách hàng {maKh}", email);
                throw new AppException("Thanh toán không thành công!");
            }

            #endregion
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutModel model, string payment = MyConstants.PAYMENT_COD)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (ModelState.IsValid)
            {
                _model = model;

                _logger.LogInformation("Nhận yêu cầu thanh toán đơn hàng của khách hàng {maKh}", email);
                var cart = await _cart.GetCartByMaKhAsync(email);
                var cartItems = await _cartItem.GetAllCartsAsync(cart.GioHangId);

                if (payment == MyConstants.PAYMENT_VNPAY)
                {
                    var vnPayModel = new VnPaymentRequestMode
                    {
                        Amount = cartItems.Sum(p => p.ThanhTien),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.HoTen} {model.SoDienThoai}",
                        FullName = model.HoTen,
                        OrderId = new Random().Next(1000, 10000),
                        Email = email,
                    };
                    var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel );
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Ok",
                        Data = paymentUrl 
                    });
                }

                _logger.LogInformation("Tạo đơn hàng cho khách hàng {maKh}", email);
                #region Tạo Đơn Hàng

                var hoaDon = new HoaDon
                {
                    MaKH = email,
                    HoTen = model.HoTen,
                    DiaChi = model.DiaChi,
                    DienThoai = model.SoDienThoai,
                    NgayDat = DateTime.Now,
                    NgayGiao = DateTime.Now.AddDays(3),
                    CachThanhToan = MyConstants.PAYMENT_COD,
                    CachVanChuyen = MyConstants.SHIPPING_WAY,
                    PhiVanChuyen = MyConstants.SHIPPING_FEE,
                    MaTrangThai = MyConstants.STATE_NEW_ORDER,
                    GhiChu = model.GhiChu,
                    TongTien = cartItems.Sum(p => p.SoLuong * p.DonGia)
                };

                await _cart.BeginTransactionAsync();
                try
                {
                    await _cart.CommitTransactionAsync();
                    await _cart.AddHoaDonAsync(hoaDon);
                    _logger.LogInformation("Thêm đơn hàng thành công");

                    _logger.LogInformation("Thêm chi tiết đơn hàng");
                    var cthds = new List<ChiTietHoaDon>();
                    foreach (var item in cartItems)
                    {
                        cthds.Add(new ChiTietHoaDon
                        {
                            MaHD = hoaDon.MaHD,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaSach = item.MaSach,
                            GiamGia = 0
                        });
                    }
                    await _cart.AddRangeChiTietHdAsync(cthds);

                    var dictionary = new Dictionary<string, int>();
                    foreach (var item in cartItems)
                    {
                        dictionary[item.MaSach] = item.SoLuong;
                    }
                    await _sach.UpdateInventoryQuantity(dictionary);

                    _logger.LogInformation("Thêm chi tiết đơn hàng thành công");
                    await _cartItem.ClearAllAsync(cart.GioHangId);
                    _logger.LogInformation("Xóa mặt hàng sau khi thanh toán của khách hàng {maKh} thành công", email);

                    _logger.LogInformation("Thanh toán cho khách hàng {maKh} thành công", email);
                    var order = _mapper.Map<HoaDonModel>(hoaDon);
                    return Ok(order);
                }
                catch (Exception ex)
                {
                    await _cart.RollbackTransactionAsync();
                    _logger.LogError("Xảy ra lỗi khi thanh toán đơn hàng cho khách hàng {maKh}", email);
                    throw new AppException("Thanh toán không thành công!");
                }
                #endregion
            }
            _logger.LogError("Dữ liệu không hợp lệ khi thanh toán đơn hàng");
            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBook(string id)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Nhận yêu cầu lấy giỏ hàng với mã KH {MaKH}", maKh);
            var cart = await _cart.GetCartByMaKhAsync(maKh) ?? await CreateCartAsync(maKh);
            _logger.LogInformation("Nhận yêu cầu lấy sách với mã sách {MaSach}", id);
            var book = await _sach.GetBookByIdAsync(id);
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

        [HttpPut("update-amount")]
        [Authorize]
        public async Task<IActionResult> UpdateAmount(string id, int amount)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Yêu cầu cập nhật số lượng sách. Mã KH: {MaKH}, Mã sách: {MaSach}, Số lượng: {Amount}", maKh, id, amount);
            var cart = await _cart.GetCartByMaKhAsync(maKh);
            var book = await _sach.GetBookByIdAsync(id);
            var cartItem = await _cartItem.GetCartItemByBookNameAsync(id, cart.GioHangId);
            await _cartItem.UpdateAsync(cartItem.GioHangChiTietId, amount);
            _logger.LogInformation("Cập nhật số lượng sách thành công. Mã sách: {BookId}, Số lượng mới: {Amount}", id, amount);
            return NoContent();
        }

        [HttpDelete("delete{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Xóa sách với mã {MaSach} từ giỏ hàng của khách hàng {CustomerId}", id, maKh);
            var cart = await _cart.GetCartByMaKhAsync(maKh);
            var cartItem = await _cartItem.GetCartItemByBookNameAsync(id, cart.GioHangId);
            await _cartItem.DeleteAsync(cartItem);
            _logger.LogInformation("Xóa thành công sách với mã {BookId} từ giỏ hàng của khách hàng {CustomerId}", id, maKh);
            return NoContent();
        }

        [HttpDelete("clear")]
        [Authorize]
        public async Task<IActionResult> ClearAll()
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Yêu cầu xóa tất cả sách trong giỏ với mã KH {id}", maKh);
            var cart = await _cart.GetCartByMaKhAsync(maKh);
            await _cartItem.ClearAllAsync(cart.GioHangId);
            _logger.LogInformation("Yêu cầu xóa tất cả sách trong giỏ với mã KH {id} thành công", maKh);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa tất cả sách thành công!",
            });
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