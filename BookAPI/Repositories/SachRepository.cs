using AutoMapper;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Database;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public async Task<IEnumerable<SachModel>> GetAllBooksAsync(string? maLoai, int page, int pageSize)
        {
            _logger.LogInformation("Xây dựng truy vấn sách với MaLoai: {maLoai}, Trang: {page}, Kích thước trang: {pageSize}", maLoai, page, pageSize);
            var listBook = _context.sachs.Include(l => l.Loai).Include(nxb => nxb.NhaXuatBan).AsQueryable();
            if (!string.IsNullOrEmpty(maLoai))
            {
                _logger.LogInformation("Truy vấn sách theo MaLoai: {maLoai}", maLoai);
                listBook = _context.sachs.Where(p => p.MaLoai == maLoai);
            }

            try
            {
                _logger.LogInformation("Số lượng sách trước phân trang: {listBook}", await listBook.CountAsync());
                var data = PaginatedList<Sach>.Create(listBook, page, pageSize);

                _logger.LogInformation("Số lượng sách sau phân trang: {data}", data.Count);
                var result = (from s in data
                              join nxb in _context.nhaXuatBans on s.MaNXB equals nxb.MaNXB
                              select new SachModel
                              {
                                  MaSach = s.MaSach,
                                  MaLoai = s.MaLoai,
                                  TenSach = s.TenSach,
                                  SoLuong = s.SoLuong,
                                  Gia = s.Gia,
                                  SoTap = s.SoTap,
                                  Anh = s.Anh,
                                  NgayNhap = s.NgayNhap,
                                  TacGia = s.TacGia,
                                  SoLuongTon = s.SoLuongTon,
                                  MoTa = s.MoTa,
                                  MaNCC = s.MaNCC,
                                  TenNhaXuatBan = nxb.TenNhaXuatBan,

                              }).OrderByDescending(p => p.NgayNhap);

                _logger.LogInformation("Truy vấn thành công lấy danh sách sách với MaLoai: {maLoai}, Trang: {page}, Kích thước trang: {pageSize}",maLoai, page, pageSize);
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
                var result = await (from s in _context.sachs
                                    join nxb in _context.nhaXuatBans on s.MaNXB equals nxb.MaNXB
                                    where s.MaSach == id.Trim()
                                    select new SachModel
                                    {
                                        MaSach = s.MaSach,
                                        MaLoai = s.MaLoai,
                                        TenSach = s.TenSach,
                                        SoLuong = s.SoLuong,
                                        Gia = s.Gia,
                                        SoTap = s.SoTap,
                                        Anh = s.Anh,
                                        NgayNhap = s.NgayNhap,
                                        TacGia = s.TacGia,
                                        SoLuongTon = s.SoLuongTon,
                                        MoTa = s.MoTa,
                                        MaNCC = s.MaNCC,
                                        TenNhaXuatBan = nxb.TenNhaXuatBan,

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
            _logger.LogInformation("Xây dựng truy vấn tìm kiếm sách với Từ khóa: {key}, Trang: {page}, Kích thước trang: {pageSize}",
                                    key, page, pageSize);
            var books = _context.sachs.Include(l => l.Loai).Include(nxb => nxb.NhaXuatBan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(key))
            {
                _logger.LogInformation("Truy vấn sách theo từ khóa: {key}", key);
                books = _context.sachs.Where(p => p.TenSach.ToLower().Contains(key.ToLower().Trim())
                    || p.TacGia.ToLower().Contains(key.ToLower().Trim()));
            }
            try
            {
                var data = PaginatedList<Sach>.Create(books, page, pageSize);

                _logger.LogInformation("Số lượng sách sau phân trang: {data}", data.Count());
                var result = (from s in data
                              join nxb in _context.nhaXuatBans on s.MaNXB equals nxb.MaNXB
                              select new SachModel
                              {
                                  MaSach = s.MaSach,
                                  MaLoai = s.MaLoai,
                                  TenSach = s.TenSach,
                                  SoLuong = s.SoLuong,
                                  Gia = s.Gia,
                                  SoTap = s.SoTap,
                                  Anh = s.Anh,
                                  NgayNhap = s.NgayNhap,
                                  TacGia = s.TacGia,
                                  SoLuongTon = s.SoLuongTon,
                                  MoTa = s.MoTa,
                                  MaNCC = s.MaNCC,
                                  TenNhaXuatBan = nxb.TenNhaXuatBan,

                              });


                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi truy vấn tìm kiếm sách với Từ khóa: {key}, Trang: {page}, Kích thước trang: {pageSize}", key, page, pageSize);
                throw;
            }
        }
        public async Task<IEnumerable<SachModel>> SearchBookByNXBAsync(string key, int page, int pageSize)
        {
            _logger.LogInformation("Xây dựng truy vấn tìm kiếm sách với theo NXB: {key}, Trang: {page}, Kích thước trang: {pageSize}",
                                    key, page, pageSize);
            var books = _context.sachs.Include(l => l.Loai).Include(nxb => nxb.NhaXuatBan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(key))
            {
                _logger.LogInformation("Truy vấn sách theo NXB: {key}", key);
                books = _context.sachs.Where(p => p.NhaXuatBan.TenNhaXuatBan.ToLower().Contains(key.ToLower().Trim()));
            }
            try
            {
                var data = PaginatedList<Sach>.Create(books, page, pageSize);

                _logger.LogInformation("Số lượng sách sau phân trang: {data}", data.Count());
                var result = (from s in data
                              join nxb in _context.nhaXuatBans on s.MaNXB equals nxb.MaNXB
                              select new SachModel
                              {
                                  MaSach = s.MaSach,
                                  MaLoai = s.MaLoai,
                                  TenSach = s.TenSach,
                                  SoLuong = s.SoLuong,
                                  Gia = s.Gia,
                                  SoTap = s.SoTap,
                                  Anh = s.Anh,
                                  NgayNhap = s.NgayNhap,
                                  TacGia = s.TacGia,
                                  SoLuongTon = s.SoLuongTon,
                                  MoTa = s.MoTa,
                                  MaNCC = s.MaNCC,
                                  TenNhaXuatBan = nxb.TenNhaXuatBan,

                              }).OrderByDescending(p => p.NgayNhap);


                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi khi truy vấn tìm kiếm sách với NXB: {key}, Trang: {page}, Kích thước trang: {pageSize}", key, page, pageSize);
                throw;
            }
        }
    }
}
