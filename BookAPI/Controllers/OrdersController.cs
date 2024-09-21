using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IHoaDonService _hoaDon;
        private readonly ILogger<OrdersController> _logger;
        private readonly IChiTietHoaDonService _hoaDonCT;

        public OrdersController(IHoaDonService hoaDon, ILogger<OrdersController> logger,
                                IChiTietHoaDonService hoaDonCT) 
        {
            _hoaDon = hoaDon;
            _logger = logger;
            _hoaDonCT = hoaDonCT;
        }
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetOrders(int? page, int? pageSize)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Yêu cầu lấy hóa đơn của khách hàng {maKh}", maKh);
                int _page = page ?? 1;
                int _pageSize = pageSize ?? 5;
                var oders = await _hoaDon.GetOrdersByMaKhAsync(maKh, _page, _pageSize);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy tất cả đơn hàng thành công",
                    Data = oders
                });
            }
        [HttpGet("order-detail/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(Guid id)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Yêu cầu lấy chi tiết hóa đơn của khách hàng {maKh} có mã HD {maHD}", maKh, order.MaHD);
                var ordersDetail = await _hoaDonCT.GetAllAsync(order.MaHD);

                _logger.LogInformation("Yêu cầu lấy chi tiết hóa đơn của khách hàng {maKh} có mã HD {maHD} thành công", maKh, order.MaHD);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy chi tiết hóa đơn thành công",
                    Data = ordersDetail
                });
            }
        [HttpPut("cancel-order/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
                _logger.LogInformation("Yêu cầu hủy đơn hàng có mã hóa đơn {maHD}", id);
                await _hoaDon.UpdateOrderStateAsync(id, MyConstants.STATE_CANCELED_ORDER);
                _logger.LogInformation("Yêu cầu hủy đơn hàng có mã hóa đơn {maHD} thành công", id);
                return NoContent();
            }
        [HttpGet("orders-confirm")]
        [Authorize(Roles = AppRole.ADMIN)]
        public async Task<IActionResult> OrdersConfirm(int? page, int? pageSize)
        {
            int _page = page ?? 1; 
            int _pageSize = pageSize ?? 5;
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
                _logger.LogInformation("Yêu cầu lấy hóa đơn xác nhận");
                var result = _hoaDon.GetOderConfirm(maKh, _page, _pageSize);
                _logger.LogInformation("Yêu cầu lấy hóa đơn xác nhận thành công");
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy thành công",
                    Data = result
                });
            }
        [HttpPut("confirm")]
        [Authorize(Roles = AppRole.ADMIN)]
        public async Task<IActionResult> Confirm(Guid id)
        {
                _logger.LogInformation("Xác nhận đơn hàng");
               await _hoaDon.UpdateOrderStateAsync(id, MyConstants.STATE_PAID);
                _logger.LogInformation("Xác nhận đơn hàng thành công");
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Xác nhận đơn hàng thành công"
                });
            }
    }
}
