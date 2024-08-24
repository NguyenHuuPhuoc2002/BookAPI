using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Repositories
{
    public class ChiTietHoaDonService : IChiTietHoaDonService
    {
        private readonly IChiTietHoaDonRepository _hoaDonCT;

        public ChiTietHoaDonService(IChiTietHoaDonRepository hoaDonCT) 
        {
            _hoaDonCT = hoaDonCT;
        }
        public async Task<IEnumerable<ChiTietHDModel>> GetAllAsync(Guid id)
        {
            return await _hoaDonCT.GetAllAsync(id);
        }
    }
}
