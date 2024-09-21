using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services;
using BookAPI.Services.Interfaces;
using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ILoaiService _loai;
        private readonly CacheService _cacheService;
        private readonly CacheSetting _cacheSetting;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ILoaiService loai, ILogger<CategoriesController> logger,
                                    CacheSetting cacheSetting, CacheService cacheService)
        {
            _loai = loai;
            _cacheService = cacheService;
            _cacheSetting = cacheSetting;
            _logger = logger;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllLoai()
        {
            string cacheKey = Caches.CacheKeyAllCategories = "Categories_All";
            _logger.LogInformation("Nhận yêu cầu HTTP lấy tất cả loại");
            var loais = _cacheService.GetCache<IEnumerable<LoaiModel>>(cacheKey);
            if (loais == null)
            {
                loais = await _loai.GetAllLoaiAsync();
                _cacheService.SetCache(cacheKey, loais, _cacheSetting.Duration, _cacheSetting.SlidingExpiration);
            }
            _logger.LogInformation("Trả về danh sách loại thành công, số lượng:{loais}", loais.Count());
            return Ok(loais);
        }
        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetLoaiById(string id)
        {
            string cacheKey = Caches.CacheKeyCategoryID = $"Category_{id}";
            _logger.LogInformation("Nhận yêu cầu HTTP lấy một đối tượng loại theo mã {id}", id);
            var loai = _cacheService.GetCache<LoaiModel>(cacheKey);
            if (loai == null)
            {
                loai = await _loai.GetLoaiByIdAsync(id);
                _cacheService.SetCache(cacheKey, loai, _cacheSetting.Duration, _cacheSetting.SlidingExpiration);
            }
            _logger.LogInformation("Trả về một đối tượng loại(nếu có) thành công");
            return Ok(loai);
        }
        [HttpPost("categories")]
        //[Authorize(Roles = AppRole.ADMIN)]
        public async Task<IActionResult> Add(LoaiModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Thông tin vào không hợp lệ");
            }
            _logger.LogInformation("Yêu cầu thêm loại");           
            var result = await _loai.AddAsync(model);
            ClearCache();
            _logger.LogInformation("Yêu cầu thêm loại thành công");
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm thành công",
                Data = model
            });
        }
        [HttpPut("update")]
        [Authorize(Roles = AppRole.ADMIN)]
        public async Task<IActionResult> Update(LoaiModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Thông tin vào không hợp lệ");
            }
            _logger.LogInformation("Yêu cầu cập nhật loại {id}", model.MaLoai);
            var result = await _loai.UpdateAsync(model);
            ClearCache();
            _logger.LogInformation("Yêu cầu cập nhật loại {id} thành công", model.MaLoai);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thành công",
                Data = model
            });
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = AppRole.ADMIN)]
        public async Task<IActionResult> Remove(string id)
        {
            _logger.LogInformation("Yêu cầu xóa loại {id}", id);
            var result = await _loai.RemoveAsync(id);
            ClearCache();
            _logger.LogInformation("Yêu cầu xóa loại {id} thành công", id);
            return NoContent();
        }

        private void ClearCache()
        {
            _cacheService.RemoveCache(Caches.CacheKeyAllCategories);
            if (Caches.CacheKeyCategoryID != null)
            {
                _cacheService.RemoveCache(Caches.CacheKeyCategoryID);
            }
        }
    }
}
