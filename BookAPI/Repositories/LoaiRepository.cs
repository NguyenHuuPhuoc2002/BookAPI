using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
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
                var loais = await _context.Loais.ToListAsync();
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
                var loai = await _context.Loais.SingleOrDefaultAsync(p => p.MaLoai == id);

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
        public async Task<bool> AddAsync(LoaiModel model)
        {
            try
            {
                var result = _mapper.Map<Loai>(model);
                _logger.LogInformation("Thực hiện thêm loại vào csdl");
                await _context.AddAsync(result);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện thêm loại vào csdl thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Thực hiện thêm loại vào csdl không thành công");
                throw;
            }
        }

        public async Task<bool> RemoveAsync(string id)
        {
            try
            {
                var loai = await _context.Loais.SingleOrDefaultAsync(p => p.MaLoai == id);
                if(loai == null)
                {
                    _logger.LogWarning("Không tìm thấy loại {id}", id);
                    return false;
                }
                _logger.LogInformation("Thực hiện xóa loại {id}", id);
                _context.Loais.Remove(loai);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện xóa loại {id}t thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Thực hiện xóa loại không thành công");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(LoaiModel model)
        {
            try
            {
                var loai = await _context.Loais.SingleOrDefaultAsync(p => p.MaLoai == model.MaLoai);
                if(loai == null)
                {
                    _logger.LogWarning("Không tìm thấy loai {maloai}", model.MaLoai);
                    return false;
                }
                var result = _mapper.Map(model, loai);
                _logger.LogInformation("Thực hiện cập nhật loai {loai}", model.MaLoai);
                _context.Update(result);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện cập nhật loai {loai} thành công", model.MaLoai);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Thực hiện cập nhập loai không thành công");
                throw;
            }
        }
    }
}
