using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Common.Exceptions;
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
                var loai = await _context.Loais.SingleOrDefaultAsync(p => p.MaLoai == id);

                if (loai == null)
                {
                    _logger.LogWarning("Không tìm thấy loại với mã {LoaiId}", id);
                    throw new KeyNotFoundException($"Không tìm thấy lọai {id}");
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
                var exist = await _context.Loais.SingleOrDefaultAsync(p => p.MaLoai == model.MaLoai ||
                                                                  p.TenLoai.ToLower().Contains(model.TenLoai.ToLower().Trim()));
                if (exist != null)
                {
                    throw new DuplicateException("Loại đã tồn tại");
                }
                var result = _mapper.Map<Loai>(model);
                _logger.LogInformation("Thực hiện thêm loại vào csdl");
                await _context.AddAsync(result);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện thêm loại vào csdl thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Thực hiện thêm loại không thành công");
                throw; 
            }
        }

        public async Task<bool> RemoveAsync(string id)
        {
            try
            {
                var loai = await _context.Loais.SingleOrDefaultAsync(p => p.MaLoai == id);
                if (loai == null)
                {
                    _logger.LogWarning("Không tìm thấy loại với mã {LoaiId}", id);
                    throw new KeyNotFoundException($"Không tìm thấy lọai {id}");
                }
                _context.Loais.Remove(loai);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện xóa loại {id}t thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Thực hiện xóa loại không thành công");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(LoaiModel model)
        {
            try
            {
                var loai = await _context.Loais.SingleOrDefaultAsync(p => p.MaLoai == model.MaLoai);
                if (loai == null)
                {
                    _logger.LogWarning("Không tìm thấy loai {maloai}", model.MaLoai);
                    throw new KeyNotFoundException($"Không tìm thấy lọai {model.MaLoai}");
                }
                var result = _mapper.Map(model, loai);
                _context.Update(result);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện cập nhật loai {loai} thành công", model.MaLoai);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Thực hiện cập nhập loai không thành công");
                throw;
            }
        }
    }
}
