using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class CartModel
    {
        public int GioHangChiTietId { get; set; }

        public string? Anh { get; set; }

        public string? TenSach { get; set; }
        public string? MaSach { get; set; }

        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGia { get; set; }
        public int GioHangId { get; set; }
    }
}
