using AutoMapper;
using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Database;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class SachRepository : ISachRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SachRepository(DataContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<SachModel>> GetAllBooksAsync(string? maLoai, int page, int pageSize)
        {
            var listBook = _context.sachs.Include(l => l.Loai).AsQueryable();
            if (!string.IsNullOrEmpty(maLoai))
            {
                listBook = _context.sachs.Where(p => p.MaLoai.Equals(maLoai));
            }
           
            var data = PaginatedList<Sach>.Create(listBook, page, pageSize);

            var result = data.OrderByDescending(p => p.NgayNhap)
                           .Select(p => new SachModel
                           {
                               MaSach = p.MaSach,
                               MaLoai = p.MaLoai,
                               TenSach = p.TenSach,
                               SoLuong = p.SoLuong,
                               Gia = p.Gia,
                               SoTap = p.SoTap,
                               Anh = p.Anh,
                               NgayNhap = p.NgayNhap,
                               TacGia = p.TacGia,
                               SoLuongTon = p.SoLuongTon,
                               MoTa = p.MoTa,
                               MaNCC = p.MaNCC,
                           });
            return result;
        }

        public async Task<SachModel> GetBookByIdAsync(string id)
        {
            var book = await _context.sachs.SingleOrDefaultAsync(p => p.MaSach == id.Trim());
            var result = _mapper.Map<SachModel>(book);
            return result;
        }

        public async Task<IEnumerable<SachModel>> SearchBookAsync(string key, int page, int pageSize)
        {
            var books = _context.sachs.Include(l => l.Loai).AsQueryable();

            if (!string.IsNullOrWhiteSpace(key))
            {
                books = _context.sachs.Where(p => p.TenSach.ToLower().Contains(key.ToLower().Trim())
                    || p.TacGia.ToLower().Contains(key.ToLower().Trim()));
            }
            var data = PaginatedList<Sach>.Create(books, page, pageSize);
            var result = data.Select(p => new SachModel
            {
                MaSach = p.MaSach,
                MaLoai = p.MaLoai,
                TenSach = p.TenSach,
                SoLuong = p.SoLuong,
                Gia = p.Gia,
                SoTap = p.SoTap,
                Anh = p.Anh,
                NgayNhap = p.NgayNhap,
                TacGia = p.TacGia,
                SoLuongTon = p.SoLuongTon,
                MoTa = p.MoTa,
                MaNCC = p.MaNCC,
            });

            return result;
        }
    }
}
