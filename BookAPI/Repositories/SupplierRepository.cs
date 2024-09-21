using AutoMapper;
using BookAPI.Data;
using BookAPI.Database;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ILogger<SupplierRepository> _logger;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SupplierRepository(DataContext context, ILogger<SupplierRepository> logger, IMapper mapper) 
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(SupplierModel model)
        {
            try
            {
                _logger.LogInformation("Thực hiện thêm nhà cung cấp");
                var supplier = _mapper.Map<NhaCungCap>(model);
                _context.NhaCungCaps.Add(supplier);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện thêm nhà cung cấp thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện thêm nhà cung cấp");
                throw;
            }
        }
        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation("Thực hiện xóa nhà cung cấp {id}", id);
                var supplier = await _context.NhaCungCaps.SingleOrDefaultAsync(p => p.MaNCC == id.Trim());
                if (supplier == null) 
                {
                    _logger.LogWarning("Không tìm thấy nhà cung cấp {id}", id);
                    return false;
                }
                _context.NhaCungCaps.Remove(supplier);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện xóa nhà cung cấp {id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện xóa nhà cung cấp");
                throw;
            }
        }
        public async Task<bool> UpdateAsync(SupplierModel model)
        {
            try
            {
                _logger.LogInformation("Thực hiện cập nhật nhà cung cấp {id}", model.MaNCC);
                var supplier = await _context.NhaCungCaps.SingleOrDefaultAsync(p => p.MaNCC == model.MaNCC.Trim());
                if (supplier == null)
                {
                    _logger.LogWarning("Không tìm thấy nhà cung cấp {id}", model.MaNCC);
                    return false;
                }
                var result = _mapper.Map(model, supplier);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Thực hiện cập nhật nhà cung cấp {id} thành công", model.MaNCC);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện cập nhật nhà cung cấp");
                throw;
            }
        }
        public async Task<IEnumerable<SupplierModel>> GetAllAsync(int page, int pageSize)
        {
            try
            {
                _logger.LogInformation("Lấy tất cả nhà cung cấp");
                var suppliers = _context.NhaCungCaps.AsQueryable();
                var pagination = PaginatedList<NhaCungCap>.Create(suppliers, page, pageSize).ToList();
                var result = pagination.Select(p => new SupplierModel
                {
                    MaNCC = p.MaNCC,
                    DiaChi = p.DiaChi,
                    DienThoai = p.DienThoai,
                    Email = p.Email,
                    MoTa = p.MoTa,
                    NguoiLienLac = p.NguoiLienLac,
                    TenCongTy = p.TenCongTy
                });
                _logger.LogInformation("Lấy tất cả nhà cung cấp thành công");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện lấy tất cả nhà cung cấp");
                throw;
            }
        }
        public async Task<SupplierModel> GetById(string id)
        {
            try
            {
                _logger.LogInformation("Thực hiện lấy nhà cung cấp {id}", id);
                var supplier = await _context.NhaCungCaps.SingleOrDefaultAsync(p => p.MaNCC == id);
                if (supplier == null)
                {
                    _logger.LogWarning("TKhông tìm thấy nhà cung cấp {id}", id);
                    return null;
                }
                var result = _mapper.Map<SupplierModel>(supplier);
                _logger.LogWarning("Thực hiện lấy nhà cung cấp {id} thành công", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện lấy 1 nhà cung cấp");
                throw;
            }
        }
        public async Task<IEnumerable<SupplierModel>> Search(string key, int page, int pageSize)
        {
            try
            {
                var suppliers = _context.NhaCungCaps.Where(p => p.TenCongTy.ToLower().Contains(key.ToLower().Trim())).AsQueryable();
                var pagination = PaginatedList<NhaCungCap>.Create(suppliers, page, pageSize).ToList();
                var result = pagination.Select(p => new SupplierModel
                {
                    MaNCC = p.MaNCC,
                    DiaChi = p.DiaChi,
                    DienThoai = p.DienThoai,
                    Email = p.Email,
                    MoTa = p.MoTa,
                    NguoiLienLac = p.NguoiLienLac,
                    TenCongTy = p.TenCongTy
                });
                _logger.LogInformation("Thực hiện tìm kiếm và lấy tất cả nhà cung cấp {key} thành công", key);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện tìm kiếm nhà cung cấp");
                throw;
            }
        }

    }
}
