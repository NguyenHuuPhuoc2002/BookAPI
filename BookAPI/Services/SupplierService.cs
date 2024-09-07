using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplier;

        public SupplierService(ISupplierRepository supplier) 
        {
            _supplier = supplier;
        }
        public async Task<bool> AddAsync(SupplierModel model)
        {
            return await _supplier.AddAsync(model);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _supplier.DeleteAsync(id);
        }

        public async Task<IEnumerable<SupplierModel>> GetAllAsync(int page, int pageSize)
        {
            return await _supplier.GetAllAsync(page, pageSize);
        }

        public async Task<SupplierModel> GetById(string id)
        {
            return await _supplier.GetById(id);
        }

        public async Task<IEnumerable<SupplierModel>> Search(string key, int page, int pageSize)
        {
            return await _supplier.Search(key, page, pageSize);
        }

        public async Task<bool> UpdateAsync(SupplierModel model)
        {
            return await _supplier.UpdateAsync(model);
        }
    }
}
