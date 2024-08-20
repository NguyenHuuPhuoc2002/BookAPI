using BookAPI.Data;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
