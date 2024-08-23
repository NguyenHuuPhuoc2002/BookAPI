using BookAPI.Data;
using BookAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories.Interfaces
{
    public interface IHoaDonRepository
    {
        Task<IEnumerable<HoaDonModel>> GetOdersAsync(string maKh, int page, int pageSize);
        Task<HoaDonModel> GetHoaDonByIdAsync(Guid id);
        
    }
}
