using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookAPI.Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PublisherRepository(DataContext context, IMapper mapper, ILogger<PublisherRepository> logger) 
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PublisherModel>> GetAllAsync(int page, int pageSize)
        {
            await Task.CompletedTask;
            try
            {
                var publishers = _context.NhaXuatBans.AsQueryable();
                var pagination = PaginatedList<NhaXuatBan>.Create(publishers, page, pageSize).ToList();
                var result = pagination.Select(p => new PublisherModel
                {
                    MaNXB = p.MaNXB,
                    DiaChi = p.DiaChi,
                    DienThoai = p.DienThoai,
                    Email = p.Email,
                    Logo = p.Logo?? "",
                    MoTa = p.MoTa,
                    NguoiLienLac = p.NguoiLienLac,
                    TenNhaXuatBan = p.TenNhaXuatBan,
                });
                _logger.LogInformation("Thực hiện truy vấn lấy tất cả nhà xuất bản trang {page} thành công", page);
                return result;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Xảy ra lỗi khi thực hiện truy vấn lấy tất cả nhà xuất bản");
                throw;
            }
        }
        public async Task<PublisherModel> GetByIdAsync(int id)
        {
            try
            {
                var publisher = await _context.NhaXuatBans.SingleOrDefaultAsync(p => p.MaNXB == id);
                if(publisher == null)
                {
                    _logger.LogWarning("Không tìm thấy nhà xuất bản {id}", id);
                    throw new KeyNotFoundException($"Không tìm thấy nhà xuất bản {id}");
                }
                var result = _mapper.Map<PublisherModel>(publisher);
                _logger.LogInformation("Thực hiện truy vấn lấy nhà xuất bản {id}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi thực hiện truy vấn lấy nhà xuất bản theo {id}", id);
                throw;
            }
        }
        public async Task<IEnumerable<PublisherModel>> SearchAsync(string key, int page, int pageSize)
        {
            await Task.CompletedTask;
            try
            {
                _logger.LogInformation("Thực hiện tìm kiếm nhà xuất bản {key}", key);
                var publishers = _context.NhaXuatBans.Where(p => p.TenNhaXuatBan.ToLower().Contains(key.ToLower().Trim()));
                var data = PaginatedList<NhaXuatBan>.Create(publishers, page, pageSize);
                var result = data.Select(p => new PublisherModel
                {
                    MaNXB = p.MaNXB,
                    DiaChi = p.DiaChi,
                    DienThoai = p.DienThoai,
                    Email = p.Email,
                    Logo = p.Logo ?? "",
                    MoTa = p.MoTa,
                    NguoiLienLac = p.NguoiLienLac,
                    TenNhaXuatBan = p.TenNhaXuatBan,
                });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện tìm kiếm nhà xuất bản {key}", key);
                throw;
            }
        }
        public async Task<bool> AddAsync(PublisherModel model)
        {
            try
            {
                var publisher = await _context.NhaXuatBans.SingleOrDefaultAsync(p => p.MaNXB == model.MaNXB ||
                                                                                p.TenNhaXuatBan.ToLower() == model.TenNhaXuatBan.ToLower().Trim());
                if (publisher != null)
                {
                    _logger.LogWarning("Nhà xuất bản đã tồn tại");
                    throw new DuplicateException("Nhà xuất bản đã tồn tại");
                }
                var result = _mapper.Map<NhaXuatBan>(model);
                await _context.NhaXuatBans.AddAsync(result);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện thêm nhà xuất bản {Name} vào csdl thành công", model.TenNhaXuatBan);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện thêm nhà xuất bản vào csdl");
                throw;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var publisher = await _context.NhaXuatBans.SingleOrDefaultAsync(p => p.MaNXB ==  id);
                if (publisher == null)
                {
                    _logger.LogWarning("Không tìm thấy nhà xuất bản {id}", id);
                    throw new KeyNotFoundException($"Không tìm thấy nhà xuất bản {id}");
                }
                var result = _mapper.Map<NhaXuatBan>(publisher);
                _context.NhaXuatBans.Remove(result);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện xóa nhà xuất bản có id {id} thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện xóa nhà xuất bản có id {id}", id);
                throw;
            }
        }
        public async Task<bool> UpdateAsync(PublisherModel model)
        {
            try
            {
                var publisher = await _context.NhaXuatBans.SingleOrDefaultAsync(p => p.MaNXB == model.MaNXB);
                if(publisher == null)
                {
                    _logger.LogWarning("Không tìm thấy nhà xuất bản có id {id}", model.MaNXB);
                    throw new KeyNotFoundException($"Không tìm thấy nhà xuất bản có id {model.MaNXB}");
                }
                var result = _mapper.Map(model, publisher);
                _context.NhaXuatBans.Update(result);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện cập nhật nhà xuất bản có id {id} thành công", model.MaNXB);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi cập nhật nhà xuất bản có id {id}", model.MaNXB);
                throw;
            }
        }
        public async Task<PublisherModel> GetByNameAsync(string key)
        {
            try
            {
                var publisher = await _context.NhaXuatBans.SingleOrDefaultAsync(p => p.TenNhaXuatBan.ToLower() == key.ToLower().Trim());
                if (publisher == null)
                {
                    _logger.LogWarning("Không tìm thấy nhà xuất bản {id}", key);
                    throw new KeyNotFoundException($"Không tìm thấy nhà xuất bản {key}");
                }
                var result = _mapper.Map<PublisherModel>(publisher);
                _logger.LogInformation("Thực hiện truy vấn lấy nhà xuất bản {id} thành công", key);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện truy vấn lấy nhà xuất bản theo {id}", key);
                throw;
            }
        }
    }
}
