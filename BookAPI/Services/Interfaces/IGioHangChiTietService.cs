using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface IGioHangChiTietService
    {
        Task<IEnumerable<CartModel>> GetAllCartsAsync(int id);
        Task<CartModel> GetCartItemByIdAsync(int id);
        Task<CartModel> GetCartItemByBookNameAsync(string maSach, int id);
        Task DeleteAsync(CartModel cartItem);
        Task UpdateAsync(int id, int amount);
        Task AddAsync(CartModel cartItem);
        Task ClearAllAsync(int id);
    }
}
