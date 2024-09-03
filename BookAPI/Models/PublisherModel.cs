using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class PublisherModel
    {
        public int MaNXB { get; set; }

        [Required]
        public string TenNhaXuatBan { get; set; }

        public string? Logo { get; set; }

        [Required]
        public string NguoiLienLac { get; set; }

        [Required, EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [MaxLength(11)]
        public string DienThoai { get; set; }

        [Required]
        [MaxLength(100)]
        public string DiaChi { get; set; }
        [MaxLength(100)]
        public string? MoTa { get; set; }
    }
}
