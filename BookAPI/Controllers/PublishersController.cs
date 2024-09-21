using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Services;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRole.ADMIN)]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisher;
        private readonly ILogger _logger;
        private readonly CacheService _cacheService;
        private readonly CacheSetting _cacheSetting;

        public PublishersController(IPublisherService publisher, ILogger<PublishersController> logger,
                                    CacheSetting cacheSetting, CacheService cacheService)
        {
            _publisher = publisher;
            _logger = logger;
            _cacheService = cacheService;
            _cacheSetting = cacheSetting;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pageSize = pageSize ?? 9;
            var cacheKey = Caches.CacheKeyAllPublishers = $"Publishers_All_{_page}_{_pageSize}";
                _logger.LogInformation("Yêu cầu lấy tất cả nhà xuất bản trang {page}", _page);
                var publishers = _cacheService.GetCache<IEnumerable<PublisherModel>>(cacheKey);
                if (publishers == null)
                {
                    publishers = await _publisher.GetAllAsync(_page, _pageSize);
                    _cacheService.SetCache(cacheKey, publishers, _cacheSetting.Duration, _cacheSetting.SlidingExpiration);
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy thành công",
                    Data = publishers
                });
            }
        [HttpGet("search")]
        public async Task<IActionResult> Search(string key, int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pageSize = pageSize ?? 9;
            var cacheKey = Caches.CacheKeyAllPublishers = $"Publishers_{key}_{_page}_{_pageSize}";
            try
            {
                _logger.LogInformation("Yêu cầu tìm kiếm nhà xuất bản theo tên {key} trang {page}", key, _page);
                if (!string.IsNullOrEmpty(key))
                {
                    var publishers = _cacheService.GetCache<IEnumerable<PublisherModel>>(cacheKey);
                    if (publishers == null)
                    {
                        publishers = await _publisher.SearchAsync(key, _page, _pageSize);
                        _cacheService.SetCache(cacheKey, publishers, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(15));
                    }
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Tìm kiếm thành công",
                        Data = publishers
                    });
                }
                else
                {
                    var publishers = _cacheService.GetCache<IEnumerable<PublisherModel>>(cacheKey);
                    if (publishers == null)
                    {
                        publishers = await _publisher.GetAllAsync(_page, _pageSize);
                        _cacheService.SetCache(cacheKey, publishers, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(15));
                    }
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Tìm kiếm thành công",
                        Data = publishers
                    });
                }
            }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cacheKey = Caches.CacheKeyAllPublishers = $"Publisher_{id}";
                _logger.LogInformation("Yêu cầu lấy nhà xuất bản theo id {id}", id);
                var result = _cacheService.GetCache<PublisherModel>(cacheKey);
                if (result == null)
                {
                    result = await _publisher.GetByIdAsync(id);
                    _cacheService.SetCache(cacheKey, result, _cacheSetting.Duration, _cacheSetting.SlidingExpiration);
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy thành công",
                    Data = result
                });
            }
        [HttpPost]
        public async Task<IActionResult> AddPublisher(PublisherModel model)
        {
                _logger.LogInformation("Yêu cầu thêm nhà xuất bản");
                if (ModelState.IsValid)
                {
                    var publisher = await _publisher.GetByNameAsync(model.TenNhaXuatBan);
                    var result = await _publisher.AddAsync(model);
                        ClearCache();
                        _logger.LogInformation("Yêu cầu thêm nhà xuất bản thành công");
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Thêm thành công",
                            Data = model
                        });
                    }
        [HttpPut]
        public async Task<IActionResult> UpdatePublisher(PublisherModel model)
        {
                _logger.LogInformation("Yêu cầu cập nhật nhà xuất bản có id {id}", model.MaNXB);
                if (ModelState.IsValid)
                {
                    var result = await _publisher.UpdateAsync(model);
                        ClearCache();
                        _logger.LogInformation("Yêu cầu cập nhật nhà xuất bản có id {id} thành công", model.MaNXB);
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Cập nhật thành công",
                            Data = model
                        });
                    }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
                _logger.LogInformation("Yêu cầu xóa nhà xuất bản có id {id}", id);
                var result = await _publisher.DeleteAsync(id);
                    ClearCache();
                    _logger.LogInformation("Yêu cầu xóa nhà xuất bản có id {id} thành công", id);
                    return NoContent();
                }
        private void ClearCache()
        {
            _cacheService.RemoveCache(Caches.CacheKeyAllPublishers);
            _cacheService.RemoveCache(Caches.CacheKeyPublisherID);
            _cacheService.RemoveCache(Caches.CacheKeyPublisherSearch);
        }
    }
}
