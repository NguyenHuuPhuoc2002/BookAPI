using AutoMapper;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Services;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRole.ADMIN)]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplier;
        private readonly ILogger<SuppliersController> _logger;
        private readonly IMapper _mapper;
        private readonly CacheService _cacheService;
        private readonly CacheSetting _cacheSetting;

        public SuppliersController(ISupplierService supplier, ILogger<SuppliersController> logger, IMapper mapper,
                                    CacheSetting cacheSetting, CacheService cacheService)
        {
            _supplier = supplier;
            _logger = logger;
            _mapper = mapper;
            _cacheService = cacheService;
            _cacheSetting = cacheSetting;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pageSize = pageSize ?? 9;
            var cacheKey = Caches.CacheKeyAllSuppliers = $"Suppliers_All_{_page}_{_pageSize}";
            _logger.LogInformation("Yêu cầu lấy tất cả nhà cung cấp");
            var suppliers = _cacheService.GetCache<IEnumerable<SupplierModel>>(cacheKey);
            if (suppliers == null)
            {
                suppliers = await _supplier.GetAllAsync(_page, _pageSize);
                _cacheService.SetCache(cacheKey, suppliers, _cacheSetting.Duration, _cacheSetting.SlidingExpiration);
            }
            _logger.LogInformation("Yêu cầu lấy tất cả nhà cung cấp thành công");
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lấy thành công",
                Data = suppliers
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var cacheKey = Caches.CacheKeySupplierID = $"Supplier_{id}";
            _logger.LogInformation("Yêu cầu lấy nhà cung cấp {id}", id);
            var supplier = _cacheService.GetCache<SupplierModel>(cacheKey);
            if (supplier == null)
            {
                supplier = await _supplier.GetById(id);
                _cacheService.SetCache(cacheKey, supplier, _cacheSetting.Duration, _cacheSetting.SlidingExpiration);
            }
            _logger.LogInformation("Yêu cầu lấy nhà cung cấp {id} thành công", id);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lấy thành công",
                Data = supplier
            });
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(string key, int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pageSize = pageSize ?? 9;
            var cacheKey = Caches.CacheKeySuppliersSearch = $"Suppliers_{key}_{_page}_{_pageSize}";
            var suppliers = _cacheService.GetCache<IEnumerable<SupplierModel>>(cacheKey);
            if (suppliers == null)
            {
                if (key == null)
                {
                    suppliers = await _supplier.GetAllAsync(_page, _pageSize);
                }
                else
                {
                    suppliers = await _supplier.Search(key, _page, _pageSize);
                }
                _cacheService.SetCache(cacheKey, suppliers, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(15));
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Tìm kiếm thành công",
                Data = suppliers
            });
        }
        [HttpPost]
        public async Task<IActionResult> AddSupplier(SupplierModel model)
        {
            _logger.LogInformation("Yêu cầu thêm nhà cung cấp");
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Thông tin vào không hợp lệ");
            }
            var supplier = await _supplier.AddAsync(model);
            ClearCache();
            _logger.LogInformation("Yêu cầu thêm nhà cung cấp thành công");
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm thành công",
                Data = model
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSupplier(string id, SupplierModel model)
        {
            _logger.LogInformation("Yêu cầu cập nhật nhà cung cấp {id}", id);
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Thông tin vào không hợp lệ");
            }
            var result = await _supplier.UpdateAsync(model);
            ClearCache();
            _logger.LogInformation("Yêu cầu cập nhật nhà cung cấp {id} thành công", id);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật thành công",
                Data = model
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(string id)
        {
            _logger.LogInformation("Yêu cầu xóa nhà cung cấp {id}", id);
            var result = await _supplier.DeleteAsync(id);
            ClearCache();
            _logger.LogInformation("Yêu cầu xóa nhà cung cấp {id} thành công", id);
            return NoContent();
        }
        private void ClearCache()
        {
            _cacheService.RemoveCache(Caches.CacheKeyAllSuppliers);
            _cacheService.RemoveCache(Caches.CacheKeySupplierID);
            _cacheService.RemoveCache(Caches.CacheKeySuppliersSearch);
        }

    }
}
