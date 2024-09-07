using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class SupplierModel
    {
        [Required]
        public string MaNCC { get; set; }
        [Required]
        public string TenCongTy { get; set; }
        [Required]
        public string NguoiLienLac { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string DienThoai { get; set; }
        [Required]
        public string DiaChi { get; set; }
        [MaxLength(50)]
        public string MoTa { get; set; }
    }
}
