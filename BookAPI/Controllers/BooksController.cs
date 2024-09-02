using AutoMapper;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ISachService _sach;
        private readonly ILogger<BooksController> _logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(ISachService sach, ILogger<BooksController> logger, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _sach = sach;
            _logger = logger;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet()]
        public async Task<IActionResult> GettAll(string? maLoai, int? page, int? pageSize)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Số trang và kích thước trang phải lớn hơn 0.",
                    });
                }
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;
                _logger.LogInformation("Nhận yêu cầu HTTP lấy sách theo mã loại {maLoai} (nếu có) còn không thì lấy danh sách sách , Trang: {pageIndex}, Kích thước trang: {pSize}", maLoai, pageIndex, pSize);
                var books = await _sach.GetAllBooksAsync(maLoai, pageIndex, pSize);

                _logger.LogInformation("Trả về danh sách sách thành công, số lượng: {books}", books.Count());
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi xử lý yêu cầu HTTP lấy tất cả sách");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Lấy toàn bộ sách theo mã sách {id}", id);
                var book = await _sach.GetBookByIdAsync(id);
                if (book != null)
                {
                    _logger.LogInformation("Lấy sách theo mã sách {id} thành công", id);
                    return Ok(book);
                }
                else
                {
                    _logger.LogError("Không tìm thấy mã sách {id}", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi lấy sách theo mã {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchBook(string keyWord, int? page, int? pageSize)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Số trang và kích thước trang phải lớn hơn 0.",
                    });
                }
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;

                _logger.LogInformation("Nhận yêu cầu HTTP lấy tất cả sách theo keyWord {keyWord}, Trang: {pageIndex}, Kích thước trang: {pSize}", keyWord, pageIndex, pSize);
                var books = await _sach.SearchBookAsync(keyWord, pageIndex, pSize);

                _logger.LogInformation("Trả về danh sách sách theo keyWord {keyWord} thành công", keyWord);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Xảy ra lỗi khi xử lý yêu cầu HTTP lấy sách theo keyWord {keyWord}", keyWord);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("search-by-publisher")]
        public async Task<IActionResult> SearchBookNXB(string keyWord, int? page, int? pageSize)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Số trang và kích thước trang phải lớn hơn 0.",
                    });
                }
                int pageIndex = page ?? 1;
                int pSize = pageSize ?? 9;

                _logger.LogInformation("Nhận yêu cầu HTTP lấy tất cả sách theo NXB {keyWord}, Trang: {pageIndex}, Kích thước trang: {pSize}", keyWord, pageIndex, pSize);
                var books = await _sach.SearchBookByNXBAsync(keyWord, pageIndex, pSize);

                _logger.LogInformation("Trả về tất cả sách theo NXB {keyWord} thành công, số lượng {books}", keyWord, books.Count());
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Xảy ra lỗi khi xử lý yêu cầu HTTP lấy sách theo NXB {keyWord}", keyWord);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromForm] SachAdminModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu thêm 1 quyển sách");
                if (ModelState.IsValid)
                {
                    var book = _mapper.Map<Sach>(model);
                    if(model.Image != null)
                    {
                        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Sach/");
                        string imageName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                        string filePath = Path.Combine(uploadDir, imageName);

                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(fs);
                        }
                        book.Anh = "Images/Sach/" + imageName;
                    }
                    else
                    {
                        book.Anh = "";
                    }
                    var result = await _sach.AddAsync(book);
                    if (result)
                    {
                        _logger.LogInformation("Yêu cầu thêm sách {TenSach} thành công", model.TenSach);
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Thêm sách thành công",
                            Data = model
                        });
                    }
                }
                _logger.LogError("Lỗi từ client");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Thông tin sách không hợp lệ",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu thêm sách không thành công");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook([FromForm] SachAdminModel model)
        {
            try
            {
                _logger.LogInformation("Yêu cầu cập nhật sách {id}", model.MaSach);
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Sach/");
                var book = await _sach.GetBookByIdAsync(model.MaSach);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách {id}", model.MaSach);
                    return NotFound(new ApiResponse
                    {
                        Success = false, 
                        Message = "Không tìm thấy sách"
                    });
                }
                if (ModelState.IsValid)
                {                   
                    if (!string.IsNullOrEmpty(book.Anh))
                    {
                        string[] split = book.Anh.Split('/');
                        string image = split[2];
                        string oldfilePath = Path.Combine(uploadsDir, image);

                        try
                        {
                            if (System.IO.File.Exists(oldfilePath))
                            {
                                System.IO.File.Delete(oldfilePath);
                            }
                        }
                        catch
                        {
                            ModelState.AddModelError("", "Xóa không thành công!");
                        }
                        if (model.Image != null)
                        {
                            
                            string imageName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                            string filePath = Path.Combine(uploadsDir, imageName);

                            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                            {
                                await model.Image.CopyToAsync(fs);
                            }
                            book.Anh = "Images/Sach/" + imageName;
                        }
                        else
                        {
                            book.Anh = "";
                        }
                    }
                    book.TenSach = model.TenSach;
                    book.Gia = model.Gia;
                    book.SoTap = model.SoTap;
                    book.NgayNhap = model.NgayNhap;
                    book.TacGia = model.TacGia;
                    book.MaLoai = model.MaLoai;
                    book.SoLuongTon = model.SoLuongTon;
                    book.MoTa = model.MoTa;
                    book.MaNCC = model.MaNCC;
                    book.MaNXB = model.MaNXB;

                    var result = _mapper.Map<Sach>(book);
                    await _sach.UpdateAsync(result);
                    _logger.LogInformation("Yêu cầu cập nhật sách {id} thành công", model.MaSach);
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Cập nhật thành công",
                        Data = book
                    });
                }
                _logger.LogError("Thông tin cập nhật sách {id} từ client không hợp lệ", model.MaSach);
                return BadRequest(new ApiResponse
                {
                    Success = true,
                    Message = "Thông tin sách không hợp lệ"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu cập nhật sách không thành công");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletee(string id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa sách {id}", id);
                var book = await _sach.GetBookByIdAsync(id);
                if(book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách {id}", id);
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy sách",
                        Data = id
                    });
                }
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Sach/");
                if (!string.IsNullOrEmpty(book.Anh))
                {
                    string[] split = book.Anh.Split('/');
                    string image = split[2];
                    string oldfilePath = Path.Combine(uploadsDir, image);

                    try
                    {
                        if (System.IO.File.Exists(oldfilePath))
                        {
                            System.IO.File.Delete(oldfilePath);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Xóa không thành công!");
                    }
                }
                var result = await _sach.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation("Yêu cầu xóa sách {id} thành công", id);
                    return NoContent();    
                }
                _logger.LogError("Lỗi từ client");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đầu vào không hợp lệ",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Yêu cầu cập nhật sách không thành công");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
