using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.RepositoryBase;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories.Interfaces
{
    public interface IHoaDonRepository 
    {
        Task<IEnumerable<HoaDonModel>> GetOrdersByMaKhAsync(string maKh, int page, int pageSize);
        Task<HoaDonModel> GetOrderByIdAsync(Guid id, string maKh);
        Task UpdateOrderStateAsync(Guid id, int state);
        Task<IEnumerable<HoaDonModel>> GetOderConfirm(string email, int page, int pageSize);
        
    }
}
