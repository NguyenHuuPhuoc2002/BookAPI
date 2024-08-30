using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class CheckoutModel
    {
        [Required]
        public string HoTen {  get; set; }
        [Required]
        public string DiaChi { get; set; }
        [Required]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải chứa từ 10 đến 11 chữ số.")]
        public string SoDienThoai { get; set; }
        [StringLength(100, ErrorMessage = "Ghi chú không được vượt quá 100 ký tự.")]
        public string? GhiChu { get; set; }

    }
}
