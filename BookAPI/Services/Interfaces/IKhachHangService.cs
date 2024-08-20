using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface IKhachHangService
    {
        Task<KhachHang> CheckLogIn(LogInModel model);
        Task<KhachHang> GetUserById(string maKH);
    }
}
