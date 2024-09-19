﻿using AutoMapper;
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
   // [Authorize(Roles = AppRole.ADMIN)]
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
            var cacheKey = Caches.CacheKeyAllSuppliers= $"Suppliers_All_{_page}_{_pageSize}";
            try
            {
                _logger.LogInformation("Yêu cầu lấy tất cả nhà cung cấp");
                var suppliers = _cacheService.GetCache<IEnumerable<SupplierModel>>(cacheKey);
                if( suppliers == null)
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Lỗi yêu cầu lấy tất cả nhà cung cấp");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var cacheKey = Caches.CacheKeySupplierID = $"Supplier_{id}";
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Lỗi yêu cầu lấy 1 nhà cung cấp");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(string key, int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pageSize = pageSize ?? 9;
            var cacheKey = Caches.CacheKeySuppliersSearch = $"Suppliers_{key}_{_page}_{_pageSize}";
            try
            {
                var suppliers = _cacheService.GetCache<IEnumerable<SupplierModel>>(cacheKey);
                if(suppliers == null)
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Lỗi yêu cầu tìm kiếm nhà cung cấp");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddSupplier(SupplierModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu thêm nhà cung cấp");
                if (ModelState.IsValid)
                {
                    var getSup = await _supplier.GetById(model.MaNCC);
                    if(getSup != null)
                    {
                        _logger.LogWarning("Đã tồn tại nhà cung cấp {id}", model.MaNCC);
                        return BadRequest(new ApiResponse
                        {
                            Success = true, 
                            Message = "Đã tồn tại nhà cung cấp"
                        });
                    }
                    var supplier = await _supplier.AddAsync(model);
                    if (!supplier)
                    {
                        _logger.LogError("Lỗi yêu cầu thêm nhà cung cấp");
                        return StatusCode(500);
                    }
                    ClearCache();
                    _logger.LogInformation("Yêu cầu thêm nhà cung cấp thành công");
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thêm thành công",
                        Data = model
                    });
                }
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Lỗi yêu cầu thêm nhà cung cấp");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(string id, SupplierModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật nhà cung cấp {id}", id);
                if (ModelState.IsValid)
                {
                    var supplier = await _supplier.GetById(id);
                    if (supplier == null)
                    {
                        _logger.LogWarning("Không tìm thấy nhà cung cấp {id}", id);
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Không tìm thấy nhà cung cấp"
                        });
                    }
                    var result = await _supplier.UpdateAsync(model);
                    if (result)
                    {
                        ClearCache();
                        _logger.LogInformation("Yêu cầu cập nhật nhà cung cấp {id} thành công", id);
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Cập nhật thành công",
                            Data = model
                        });
                    }
                    return StatusCode(500);
                }
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Lỗi yêu cầu thêm nhà cung cấp");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(string id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa nhà cung cấp {id}", id);

                var supplier = await _supplier.GetById(id);
                if (supplier == null)
                {
                    _logger.LogWarning("Không tìm thấy nhà cung cấp {id}", id);
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy nhà cung cấp"
                    });
                }
                var result = await _supplier.DeleteAsync(id);
                if (result)
                {
                    ClearCache();
                    _logger.LogInformation("Yêu cầu xóa nhà cung cấp {id} thành công", id);
                    return NoContent();
                }
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Lỗi yêu cầu xóa nhà cung cấp");
                return StatusCode(500, ex.Message);
            }
        }

        private void ClearCache()
        {
            _cacheService.RemoveCache(Caches.CacheKeyAllSuppliers);
            _cacheService.RemoveCache(Caches.CacheKeySupplierID);
            _cacheService.RemoveCache(Caches.CacheKeySuppliersSearch);
        }

    }
}
