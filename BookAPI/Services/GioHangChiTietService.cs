using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class GioHangChiTietService : IGioHangChiTietService
    {
        private readonly IGioHangChiTietRepository _cartItem;

        public GioHangChiTietService(IGioHangChiTietRepository cartItem) 
        {
            _cartItem = cartItem;
        }
        public async Task AddAsync(CartModel cartItem)
        {
            await _cartItem.AddAsync(cartItem);
        }

        public async Task ClearAllAsync(int id)
        {
            await _cartItem.ClearAllAsync(id);
        }

        public async Task DeleteAsync(CartModel cartItem)
        {
            await _cartItem.DeleteAsync(cartItem);
        }

        public async Task<IEnumerable<CartModel>> GetAllCartsAsync(int id)
        {
            return await _cartItem.GetAllCartsAsync(id);
        }

        public async Task<CartModel> GetCartItemByBookNameAsync(string maSach, int id)
        {
            return await _cartItem.GetCartItemByBookNameAsync(maSach, id);
        }

        public async Task<CartModel> GetCartItemByIdAsync(int id)
        {
            return await _cartItem.GetCartItemByIdAsync(id);
        }

        public async Task UpdateAsync(int id, int amount)
        {
            await _cartItem.UpdateAsync(id, amount);
        }
    }
}
