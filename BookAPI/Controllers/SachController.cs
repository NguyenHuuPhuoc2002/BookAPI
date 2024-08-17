using BookAPI.Data;
using BookAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SachController : ControllerBase
    {
        private readonly ISachRepository _sach;
        private readonly ILogger<SachController> _logger;

        public SachController(ISachRepository sach, ILogger<SachController> logger)
        {
            _sach = sach;
            _logger = logger;
        }

        [HttpGet("books")]
        public async Task<IActionResult> GettAll(string? maLoai, int? page, int? pageSize)
        {
            try
            {
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;
                _logger.LogInformation($"Lấy sách theo mã loại {maLoai} (nếu có) còn không thì lấy toàn bộ sách , Trang: {pageIndex}, Kích thước trang: {pSize}");
                var books = await _sach.GetAllBooksAsync(maLoai, pageIndex, pSize);

                _logger.LogInformation($"Lấy sách thành công, tổng số sách: {books.Count()}");
                return Ok(books);
            }
            catch
            {
                _logger.LogError($"Lỗi khi lấy sách ");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("books/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                _logger.LogInformation($"Lấy toàn bộ sách theo mã sách {id}");
                var book = await _sach.GetBookByIdAsync(id);
                if (book != null)
                {
                    _logger.LogInformation($"Lấy sách theo mã sách {id} thành công");
                    return Ok(book);
                }
                else
                {
                    _logger.LogError($"Không tìm thấy mã sách {id} : {DateTime.Now}");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy sách theo mã {id} :  {DateTime.Now}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("books/search")]
        public async Task<IActionResult> SearchBook(string keyWord, int? page, int? pageSize)
        {
            try
            {
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;

                _logger.LogInformation($"Lấy toàn bộ sách theo keyWord {keyWord}, Trang: {pageIndex}, Kích thước trang: {pSize}");
                var books = await _sach.SearchBookAsync(keyWord, pageIndex, pSize);

                _logger.LogInformation($"Lấy toàn bộ sách theo keyWord {keyWord} thành công");
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Lỗi khi lấy sách theo keyWord {keyWord}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("books/search-nhaxuatban")]
        public async Task<IActionResult> SearchBookNXB(string keyWord, int? page, int? pageSize)
        {
            try
            {
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;

                _logger.LogInformation($"Lấy toàn bộ sách theo NXB {keyWord}, Trang: {pageIndex}, Kích thước trang: {pSize}");
                var books = await _sach.SearchBookByNXBAsync(keyWord, pageIndex, pSize);

                _logger.LogInformation($"Lấy toàn bộ sách theo NXB {keyWord} thành công");
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Lỗi khi lấy sách theo NXB {keyWord}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
