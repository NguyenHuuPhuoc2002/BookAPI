using AutoMapper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class LoaiRepository : ILoaiRepository
    {
        private readonly DataContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public LoaiRepository(DataContext context, ILogger<LoaiRepository> logger, IMapper mapper) 
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IEnumerable<LoaiModel>> GetAllLoaiAsync()
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy tất cả loại");
                var loais = await _context.loais.ToListAsync();
                var result = loais.Select(p => new LoaiModel
                {
                    MaLoai = p.MaLoai,
                    TenLoai = p.TenLoai,
                });
                _logger.LogInformation("Truy vấn lấy tất cả loại thành công, Số lượng : {result} ", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi truy vấn lấy tất cả loại");
                throw;
            }
        }

        public async Task<LoaiModel> GetLoaiByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Truy vấn lấy loại theo mã {id}", id);
                var loai = await _context.loais.SingleOrDefaultAsync(p => p.MaLoai == id);

                if (loai == null)
                {
                    _logger.LogWarning("Không tìm thấy loại với mã {LoaiId}", id);
                }

                var result = _mapper.Map<LoaiModel>(loai);
                _logger.LogInformation("Truy vấn lấy loại theo mã {id} thành công", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi truy vấn lấy loại theo mã {id}", id);
                throw;
            }
        }
    }
}
