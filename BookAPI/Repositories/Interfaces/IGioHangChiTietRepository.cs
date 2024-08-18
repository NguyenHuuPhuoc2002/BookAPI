using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Repositories.Interfaces
{
    public interface IGioHangChiTietRepository
    {
        Task<IEnumerable<CartModel>> GetAllCartsAsync();
        Task<CartModel> GetCartItemByIdAsync(int id);
        Task<CartModel> GetCartItemByBookNameAsync(string maSach, int id);
        Task DeleteAsync(int id);
        Task UpdateAsync(int id);
        Task AddAsync(CartModel cartItem);
    }
}
