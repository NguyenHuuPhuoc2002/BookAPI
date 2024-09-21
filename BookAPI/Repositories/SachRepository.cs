using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookAPI.Repositories
{
    public class SachRepository : ISachRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SachRepository> _logger;

        public SachRepository(DataContext context, IMapper mapper, ILogger<SachRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddAsync(Sach model)
        {
            try
            {
                await _context.AddAsync(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện thêm sách {id} thành công", model.TenSach);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện thêm sách");
                throw ex;
            }
        }
        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var book = await _context.Sachs.SingleOrDefaultAsync(p => p.MaSach == id);
                if (book == null)
                {
                    _logger.LogWarning("Không tìm thấy sách {id}", id);
                    return false;
                }
                _context.Sachs.Remove(book);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện xóa sách {id} thành công", book.TenSach);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện xóa sách");
                throw ex;
            }
        }
        public async Task<bool> UpdateAsync(Sach model)
        {
            try
            {
                _context.Sachs.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện cập nhật sách thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện cập nhật sách");
                throw ex;
            }
        }
        public async Task<IEnumerable<SachModel>> GetAllBooksAsync(string? maLoai, int page, int pageSize)
        {
            try
            {
                _logger.LogInformation("Xây dựng truy vấn sách với MaLoai: {maLoai}, Trang: {page}, Kích thước trang: {pageSize}", maLoai, page, pageSize);

                var listBook = _context.Sachs
                    .Include(l => l.Loai) 
                    .Include(nxb => nxb.NhaXuatBan) 
                    .Include(ncc => ncc.NhaCungCap) 
                    .AsQueryable();

                if (!string.IsNullOrEmpty(maLoai))
                {
                    _logger.LogInformation("Truy vấn sách theo MaLoai: {maLoai}", maLoai);
                    listBook = listBook.Where(p => p.MaLoai == maLoai);
                }

                listBook = listBook.OrderByDescending(p => p.NgayNhap);

                _logger.LogInformation("Số lượng sách trước phân trang: {listBook}", await listBook.CountAsync());
                var data = PaginatedList<Sach>.Create(listBook, page, pageSize).ToList();

                _logger.LogInformation("Số lượng sách sau phân trang: {data}", data.Count());

                var result = data.Select(s => new SachModel
                {
                    MaSach = s.MaSach,
                    MaLoai = s.MaLoai,
                    TenSach = s.TenSach,
                    Gia = s.Gia,
                    SoTap = s.SoTap,
                    Anh = s.Anh,
                    NgayNhap = s.NgayNhap,
                    TacGia = s.TacGia,
                    SoLuongTon = s.SoLuongTon,
                    MoTa = s.MoTa,
                    MaNCC = s.MaNCC,
                    MaNXB = s.MaNXB,
                    TenLoai = s.Loai?.TenLoai,
                    TenNhaXuatBan = s.NhaXuatBan?.TenNhaXuatBan,
                    TenNhaCungCap = s.NhaCungCap?.TenCongTy 
                });

                _logger.LogInformation("Truy vấn thành công lấy danh sách sách với MaLoai: {maLoai}, Trang: {page}, Kích thước trang: {pageSize}", maLoai, page, pageSize);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi truy vấn lấy danh sách sách với MaLoai: {maLoai}, Trang: {page}, Kích thước trang: {pageSize}", maLoai, page, pageSize);
                throw;
            }
        }
        public async Task<SachModel> GetBookByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy sách với ID {id} ", id);
                var result = await (from s in _context.Sachs
                                    where s.MaSach == id.Trim()
                                    select new SachModel
                                    {
                                        MaSach = s.MaSach,
                                        MaLoai = s.MaLoai,
                                        TenSach = s.TenSach,
                                        Gia = s.Gia,
                                        SoTap = s.SoTap,
                                        Anh = s.Anh,
                                        NgayNhap = s.NgayNhap,
                                        TacGia = s.TacGia,
                                        SoLuongTon = s.SoLuongTon,
                                        MoTa = s.MoTa,
                                        MaNCC = s.MaNCC,
                                        MaNXB = s.MaNXB,
                                        TenLoai = s.Loai.TenLoai,
                                        TenNhaXuatBan = s.NhaXuatBan.TenNhaXuatBan,
                                        TenNhaCungCap = s.NhaCungCap.TenCongTy

                                    }).SingleOrDefaultAsync();
                if (result == null)
                {
                    _logger.LogWarning("Truy vấn lấy sách với ID {id} không tìm thấy.", id);
                }
                else
                {
                    _logger.LogInformation("Truy vấn thành công sách với ID {id}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi truy vấn sách với ID {id} ", id);
                throw;
            }
        }
        public async Task<IEnumerable<SachModel>> SearchBookAsync(string key, int page, int pageSize)
        {
            var books = _context.Sachs.AsQueryable();
            try
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogInformation("Truy vấn sách theo từ khóa: {key}", key);
                    books = _context.Sachs
                            .Include(l => l.Loai)
                            .Include(nxb => nxb.NhaXuatBan)
                            .Include(ncc => ncc.NhaCungCap)
                            .Where(p => p.TenSach.ToLower().Contains(key.ToLower().Trim())|| p.TacGia.ToLower().Contains(key.ToLower().Trim()));
                    books = books.OrderByDescending(p => p.NgayNhap);
                    var data = PaginatedList<Sach>.Create(books, page, pageSize);

                    _logger.LogInformation("Số lượng sách sau phân trang: {data}", data.Count());
                    var result = data.Select(s => new SachModel
                    {
                        MaSach = s.MaSach,
                        MaLoai = s.MaLoai,
                        TenSach = s.TenSach,
                        Gia = s.Gia,
                        SoTap = s.SoTap,
                        Anh = s.Anh,
                        NgayNhap = s.NgayNhap,
                        TacGia = s.TacGia,
                        SoLuongTon = s.SoLuongTon,
                        MoTa = s.MoTa,
                        MaNCC = s.MaNCC,
                        MaNXB = s.MaNXB,
                        TenLoai = s.Loai?.TenLoai,
                        TenNhaXuatBan = s.NhaXuatBan?.TenNhaXuatBan,
                        TenNhaCungCap = s.NhaCungCap?.TenCongTy
                    });
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi truy vấn tìm kiếm sách với Từ khóa: {key}, Trang: {page}, Kích thước trang: {pageSize}", key, page, pageSize);
                throw;
            }
        }
        public async Task<IEnumerable<SachModel>> SearchBookByNXBAsync(string key, int page, int pageSize)
        {
            try
            {
                _logger.LogInformation("Xây dựng truy vấn tìm kiếm sách với theo NXB: {key}, Trang: {page}, Kích thước trang: {pageSize}",
                                    key, page, pageSize);
                var books = _context.Sachs.AsQueryable();

                if (!string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogInformation("Truy vấn sách theo NXB: {key}", key);
                    books = _context.Sachs
                            .Include(l => l.Loai)
                            .Include(nxb => nxb.NhaXuatBan)
                            .Include(ncc => ncc.NhaCungCap)
                            .Where(p => p.NhaXuatBan.TenNhaXuatBan.ToLower().Contains(key.ToLower().Trim()));
                }
                books = books.OrderByDescending(p => p.NgayNhap);
                var data = PaginatedList<Sach>.Create(books, page, pageSize);

                _logger.LogInformation("Số lượng sách sau phân trang: {data}", data.Count());
                var result = data.Select(s => new SachModel
                {
                    MaSach = s.MaSach,
                    MaLoai = s.MaLoai,
                    TenSach = s.TenSach,
                    Gia = s.Gia,
                    SoTap = s.SoTap,
                    Anh = s.Anh,
                    NgayNhap = s.NgayNhap,
                    TacGia = s.TacGia,
                    SoLuongTon = s.SoLuongTon,
                    MoTa = s.MoTa,
                    MaNCC = s.MaNCC,
                    MaNXB = s.MaNXB,
                    TenLoai = s.Loai?.TenLoai,
                    TenNhaXuatBan = s.NhaXuatBan?.TenNhaXuatBan,
                    TenNhaCungCap = s.NhaCungCap?.TenCongTy
                });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi truy vấn tìm kiếm sách với NXB: {key}, Trang: {page}, Kích thước trang: {pageSize}", key, page, pageSize);
                throw;
            }
        }
        public async Task UpdateInventoryQuantity(Dictionary<string, int> books)
        {
            var bookIds = books.Keys.ToList();
            try
            {
                var booksToUpdate = await _context.Sachs
                                            .Where(b => bookIds.Contains(b.MaSach))
                                            .ToListAsync();
                foreach (var book in booksToUpdate)
                {
                    // Nếu mã sách (book.MaSach) tồn tại trong dictionary, cập nhật số lượng tồn kho
                    if (books.TryGetValue(book.MaSach, out int quantity))
                    {
                        book.SoLuongTon -= quantity;

                        if (book.SoLuongTon < 0)
                        {
                            _logger.LogWarning("Số lượng tồn của sách {maSach} đã hết", book.MaSach);
                            throw new Exception($"Số lượng tồn kho không đủ cho sách với mã ID {book.MaSach}. Số lượng tồn kho hiện tại: {book.SoLuongTon}");
                        }
                    }
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện cập nhật số lượng tồn thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi thực hiện cập nhật số lượng tồn");
                throw;
            }
        }
    }
}
