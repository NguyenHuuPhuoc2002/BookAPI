using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class HoaDonService : IHoaDonService
    {
        private readonly IHoaDonRepository _hoaDon;

        public HoaDonService(IHoaDonRepository hoaDon)
        {
            _hoaDon = hoaDon;
        }
        public async Task<HoaDonModel> GetOrderByIdAsync(Guid id, string maKh)
        {
            return await _hoaDon.GetOrderByIdAsync(id , maKh);
        }

        public async Task<IEnumerable<HoaDonModel>> GetOrdersByMaKhAsync(string maKh, int page, int pageSize)
        {
            return await _hoaDon.GetOrdersByMaKhAsync(maKh, page, pageSize);
        }
    }
}
