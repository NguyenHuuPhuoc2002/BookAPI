﻿using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiController : ControllerBase
    {
        private readonly ILoaiService _loai;
        private readonly ILogger<LoaiController> _logger;

        public LoaiController(ILoaiService loai, ILogger<LoaiController> logger) 
        {
            _loai = loai;
            _logger = logger;
        }

        [HttpGet("loais")]
        public async Task<IActionResult> GetAllLoai() 
        {
            try
            {
                _logger.LogInformation("Nhận yêu cầu HTTP lấy tất cả loại");
                var loais = await _loai.GetAllLoaiAsync();

                _logger.LogInformation("Trả về danh sách loại thành công, số lượng:{loais}", loais.Count());
                return Ok(loais);
            }catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi xử lý yêu cầu HTTP lấy tất cả loại");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoaiById(string id)
        {
            try
            {
                _logger.LogInformation("Nhận yêu cầu HTTP lấy một đối tượng loại theo mã {id}", id);
                var loai = await _loai.GetLoaiByIdAsync(id);

                _logger.LogInformation("Trả về một đối tượng loại(nếu có) thành công");
                return Ok(loai);
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi xử lý yêu cầu HTTP lấy một đối tượng loại");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add(LoaiModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu thêm loại");
                var loai = await _loai.GetLoaiByIdAsync(model.MaLoai);
                if (loai != null)
                {
                    _logger.LogWarning("Loại {LoaiId} đã tồn tại", model.MaLoai);
                    return BadRequest(new ApiResponse
                    {
                        Success = true,
                        Message = "Đã tồn tại mã loại này",
                        Data = loai
                    });
                }
                var result = await _loai.AddAsync(model);
                if (result)
                {
                    _logger.LogInformation("Yêu cầu thêm loại thành công");
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thêm thành công",
                        Data = model
                    });
                }
                _logger.LogInformation("Yêu cầu thêm loại không thành công");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Thêm không thành công",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu thêm loại không thành công");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("update")] 
        public async Task<IActionResult> Update(LoaiModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật loại {id}", model.MaLoai);
                var result = await _loai.UpdateAsync(model);
                if (result)
                {
                    _logger.LogInformation("Yêu cầu cập nhật loại {id} thành công", model.MaLoai);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Cập nhật thành công",
                        Data = model
                    });
                }
                _logger.LogInformation("Yêu cầu cập nhật loại {id} không thành công", model.MaLoai);
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Cập nhật không thành công",
                    Data = null
                });
            }
            catch(Exception ex)
            {
                _logger.LogError("Yêu cầu cập nhật loai {id} không thành công", model.MaLoai);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Remove(string id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa loại {id}", id);
                var result = await _loai.RemoveAsync(id);
                if (result)
                {
                    _logger.LogInformation("Yêu cầu xóa loại {id} thành công", id);
                    return NoContent();
                }
                _logger.LogInformation("Yêu cầu xóa loại {id} không thành công", id);
                return BadRequest(new ApiResponse
                {
                    Success = true,
                    Message = "Xóa không thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu xóa loại không thành công {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
