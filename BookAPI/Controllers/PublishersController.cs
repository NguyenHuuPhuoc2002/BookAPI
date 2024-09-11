using BookAPI.Helper;
using BookAPI.Models;
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

        public PublishersController(IPublisherService publisher, ILogger<PublishersController> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pageSize = page ?? 5;
            try
            {
                _logger.LogInformation("Yêu cầu lấy tất cả nhà xuất bản trang {page}", _page);
                var result = await _publisher.GetAllAsync(_page, _pageSize);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu lấy tất cả nhà xuất bản không thành công");
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
                _logger.LogInformation("Yêu cầu tìm kiếm nhà xuất bản theo tên {key} trang {page}", key, _page);
                if (!string.IsNullOrEmpty(key))
                {
                    var result = await _publisher.SearchAsync(key, _page, _pageSize);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Tìm kiếm thành công",
                        Data = result
                    });
                }
                else
                {
                    var result = await _publisher.GetAllAsync(_page, _pageSize);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Tìm kiếm thành công",
                        Data = result
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu tìm kiếm nhà xuất bản xảy ra lỗi");
                return (StatusCode(500, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy nhà xuất bản theo id {id}", id);
                var result = await _publisher.GetByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Không tìm thất nhà xuất bản có id {id}", id);
                    return NotFound();
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu lấy nhà xuất bản theo id {id}", id);
                return (StatusCode(500, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPublisher(PublisherModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu thêm nhà xuất bản");
                if (ModelState.IsValid)
                {
                    var publisher = await _publisher.GetByNameAsync(model.TenNhaXuatBan);
                    if (publisher != null)
                    {
                        _logger.LogWarning("Đã tồn tại nhà xuất bản {Name}", model.TenNhaXuatBan);
                        return BadRequest(new ApiResponse
                        {
                            Success = false,
                            Message = "Đã tồn tại nhà xuất bản này"
                        });
                    }
                    var result = await _publisher.AddAsync(model);
                    if (result)
                    {
                        _logger.LogInformation("Yêu cầu thêm nhà xuất bản thành công");
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Thêm thành công",
                            Data = model
                        });
                    }
                    _logger.LogInformation("Yêu cầu thêm nhà xuất bản không thành công");
                    return StatusCode(500);
                }
                _logger.LogInformation("Đầu vào không hợp lệ");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu thêm nhà xuất bản xảy ra lỗi");
                return (StatusCode(500, ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePublisher(PublisherModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật nhà xuất bản có id {id}", model.MaNXB);
                if (ModelState.IsValid)
                {
                    var result = await _publisher.UpdateAsync(model);
                    if (result)
                    {
                        _logger.LogInformation("Yêu cầu cập nhật nhà xuất bản có id {id} thành công", model.MaNXB);
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Cập nhật thành công",
                            Data = model
                        });
                    }
                    _logger.LogInformation("Yêu cầu cập nhật nhà xuất bản có id {id} không thành công", model.MaNXB);
                    return StatusCode(500);
                }
                _logger.LogInformation("Đầu vào không hợp lệ");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu cập nhật nhà xuất bản xảy ra lỗi");
                return (StatusCode(500, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa nhà xuất bản có id {id}", id);
                var publisher = await _publisher.GetByIdAsync(id);
                if(publisher == null)
                {
                    _logger.LogWarning("Không tìm thấy nhà xuất bản có id {id}", id);
                    return NotFound();
                }
                var result = await _publisher.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation("Yêu cầu xóa nhà xuất bản có id {id} thành công", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogInformation("Yêu cầu xóa nhà xuất bản có id {id} không thành công", id);
                    return StatusCode(500);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu xóa nhà xuất bản xảy ra lỗi");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
