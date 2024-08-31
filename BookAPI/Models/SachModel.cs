using BookAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class SachModel
    {
        [Required]
        public string MaSach { get; set; }
        [Required]
        public string TenSach { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Giág phải lớn hơn 0.")]
        public double? Gia { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Số tập phải lớn hơn 0.")]
        public int? SoTap { get; set; }
        public DateTime? NgayNhap { get; set; }
        [Required]
        public string TacGia { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int? SoLuongTon { get; set; }
        public string? MoTa { get; set; }
        public string? MaNCC { get; set; }
        [Required]
        public string TenNhaXuatBan{ get; set; }
    }
}
