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

        public SachController(ISachRepository sach)
        {
            _sach = sach;
        }

        [HttpGet("books")]
        public async Task<IActionResult> GettAll(string? maLoai, int? page, int? pageSize)
        {
            try
            {
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;
                var books = await _sach.GetAllBooksAsync(maLoai, pageIndex, pSize);
                return Ok(books);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("books/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var book = await _sach.GetBookByIdAsync(id);
                if (book != null)
                {
                    return Ok(book);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("books/search")]
        public async Task<IActionResult> SearchBook(string? keyWord, int? page, int? pageSize)
        {
            try
            {
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;
                var books = await _sach.SearchBookAsync(keyWord, pageIndex, pSize);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
