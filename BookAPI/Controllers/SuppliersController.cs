using AutoMapper;
using BookAPI.Models;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplier;
        private readonly ILogger<SuppliersController> _logger;
        private readonly IMapper _mapper;

        public SuppliersController(ISupplierService supplier, ILogger<SuppliersController> logger, IMapper mapper)
        {
            _supplier = supplier;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pageSize = pageSize ?? 5;
            try
            {
                _logger.LogInformation("Yêu cầu lấy tất cả nhà cung cấp");
                var suppliers = await _supplier.GetAllAsync(_page, _pageSize);
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
            try
            {
                _logger.LogInformation("Yêu cầu lấy nhà cung cấp {id}", id);
                var supplier = await _supplier.GetById(id);
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
            int _pageSize = pageSize ?? 5;
            try
            {
                IEnumerable<SupplierModel> suppliers;
                if (key == null)
                {
                    suppliers = await _supplier.GetAllAsync(_page, _pageSize);
                }
                else
                {
                    suppliers = await _supplier.Search(key, _page, _pageSize);
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
                    await _supplier.UpdateAsync(model);
                    _logger.LogInformation("Yêu cầu cập nhật nhà cung cấp {id} thành công", id);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Cập nhật thành công",
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
                await _supplier.DeleteAsync(id);
                _logger.LogInformation("Yêu cầu xóa nhà cung cấp {id} thành công", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Lỗi yêu cầu xóa nhà cung cấp");
                return StatusCode(500, ex.Message);
            }
        }

    }
}
