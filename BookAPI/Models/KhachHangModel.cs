using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class KhachHangModel
    {
        [Display(Name = "Tên đăng nhập")]
        public string MaKH { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public int? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
        public string Email { get; set; }
        public IFormFile Image { get; set; }

    }
}
