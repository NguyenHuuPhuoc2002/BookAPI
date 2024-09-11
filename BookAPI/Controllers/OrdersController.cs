﻿using BookAPI.Helper;
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

        [HttpPost("orders")]
        [Authorize]
        public async Task<IActionResult> GetOrders(int? page, int? pageSize)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Yêu cầu lấy hóa đơn của khách hàng {maKh}", maKh);
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Số trang và kích thước trang phải lớn hơn 0.",
                    });
                }
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu lấy hóa đơn không thành công");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("order-detail/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(Guid id)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            try
            {
                var order = await _hoaDon.GetOrderByIdAsync(id, maKh);
                if (order == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng của khách hàng {maKh} với mã HD {maHD}", maKh, id);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"Không tìm thấy đơn hàng của khách hàng {maKh} với mã HD {id}"
                    });
                }
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu lấy chi tiết hóa đơn không thành công");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("cancel-order/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu hủy đơn hàng có mã hóa đơn {maHD}", id);
                await _hoaDon.UpdateOrderStateAsync(id, MyConstants.STATE_CANCELED_ORDER);
                _logger.LogInformation("Yêu cầu hủy đơn hàng có mã hóa đơn {maHD} thành công", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi hủy đơn hàng");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                });
            }
        }

        [HttpGet("orders-confirm")]
        [Authorize(Roles = AppRole.ADMIN)]
        public async Task<IActionResult> OrdersConfirm(int? page, int? pageSize)
        {
            int _page = page ?? 1; 
            int _pageSize = pageSize ?? 5;
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu lấy đơn hàng chờ xác nhận xảy ra lỗi");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("confirm")]
        [Authorize(Roles = AppRole.ADMIN)]
        public async Task<IActionResult> Confirm(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xác nhận đơn hàng không thành công");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
