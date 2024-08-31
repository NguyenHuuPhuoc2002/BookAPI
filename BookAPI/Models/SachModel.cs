using BookAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class SachModel
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public double? Gia { get; set; }
        public int? SoTap { get; set; }
        public string? Anh { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string TacGia { get; set; }
        public string MaLoai { get; set; }
        public int? SoLuongTon { get; set; }
        public string? MoTa { get; set; }
        public int? MaNXB { get; set; }
        public string? MaNCC { get; set; }
        public string TenNhaXuatBan{ get; set; }
    }
    public class SachAdminModel
    {
        [Required]
        public string MaSach { get; set; }
        [Required]
        public string TenSach { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Giág phải lớn hơn 0.")]
        [Required]
        public double? Gia { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số tập phải lớn hơn 0.")]
        public int? SoTap { get; set; }
        public DateTime? NgayNhap { get; set; }
        [Required]
        public string TacGia { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int? SoLuongTon { get; set; }
        public string? MoTa { get; set; }
        [Required]
        public string MaLoai { get; set; }
        [Required]
        public string? MaNCC { get; set; }
        [Required]
        public int? MaNXB { get; set; }
        public IFormFile? Image { get; set; }
    }
}
