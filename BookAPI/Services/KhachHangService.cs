using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangRepository _khachHang;

        public KhachHangService(IKhachHangRepository khachHang)
        {
            _khachHang = khachHang;
        }
        public async Task<KhachHang> CheckLogIn(LogInModel model)
        {
            return await _khachHang.CheckLogIn(model);
        }

        public async Task<KhachHang> GetUserById(string maKH)
        {
            return await _khachHang.GetUserById(maKH);
        }
    }
}
