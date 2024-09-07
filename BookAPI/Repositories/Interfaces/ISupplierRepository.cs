using BookAPI.Models;

namespace BookAPI.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<bool> AddAsync(SupplierModel model);
        Task<bool> DeleteAsync(string id);
        Task<bool> UpdateAsync(SupplierModel model);
        Task<IEnumerable<SupplierModel>> GetAllAsync(int page, int pageSize);
        Task<SupplierModel> GetById(string id);
        Task<IEnumerable<SupplierModel>> Search(string key, int page, int pageSize);
    }
}
