using BookAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class SachModel
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public int? SoLuong { get; set; }
        public double? Gia { get; set; }
        public int? SoTap { get; set; }
        public string? Anh { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string? TacGia { get; set; }
        public string? MaLoai { get; set; }
        public int? SoLuongTon { get; set; }
        public string? MoTa { get; set; }
        public string? MaNCC { get; set; }
    }
}
