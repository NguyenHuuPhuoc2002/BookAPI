using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HoaDonRepository> _logger;

        public HoaDonRepository(DataContext context, IMapper mapper, ILogger<HoaDonRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<HoaDonModel> GetOrderByIdAsync(Guid id, string maKh)
        {
            var order = await _context.HoaDons.SingleOrDefaultAsync(hd => hd.MaHD == id && hd.MaKH == maKh);
            var result = _mapper.Map<HoaDonModel>(order);
            return result;
        }

        public async Task<IEnumerable<HoaDonModel>> GetOrdersByMaKhAsync(string maKh,int page, int pageSize)
        {
            try
            {
                _logger.LogInformation("Thực hiện truy vấn lấy hóa đơn của {maKh}", maKh);
                var initData = _context.HoaDons.OrderByDescending(p => p.NgayDat).AsQueryable();
                var query = from hd in initData
                            join tt in _context.TrangThais on hd.MaTrangThai equals tt.MaTrangThai
                            where hd.MaKH == maKh
                            select new HoaDonModel
                            {
                                MaNV = hd.MaNV,
                                MaKH = hd.MaKH,
                                NgayDat = hd.NgayDat,
                                NgayGiao = hd.NgayGiao,
                                HoTen = hd.HoTen,
                                DiaChi = hd.DiaChi,
                                DienThoai = hd.DienThoai,
                                CachThanhToan = hd.CachThanhToan,
                                PhiVanChuyen = MyConstants.SHIPPING_FEE,
                                TrangThai = tt.TenTrangThai,
                                MaTrangThai = hd.MaTrangThai,
                                MaHD = hd.MaHD,
                                GhiChu = hd.GhiChu ?? "",
                                CachVanChuyen = hd.CachVanChuyen,
                            };
                _logger.LogInformation("Danh sách hóa đơn của {maKh}, Số lượng {Count}", maKh, query.Count());
                var result = PaginatedList<HoaDonModel>.Create(query.AsNoTracking(), page, pageSize);
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi lấy tất cả hóa đơn");
                throw;
            }
        }
    }
}
