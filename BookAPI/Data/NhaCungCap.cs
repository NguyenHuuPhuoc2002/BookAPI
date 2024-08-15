using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("NhaCungCap")]
    public class NhaCungCap
    {
        [Key]
        [StringLength(50)]
        public string MaNCC { get; set; }

        [StringLength(50)]
        public string TenCongTy { get; set; }

        [StringLength(100)]
        public string? Logo { get; set; }

        [StringLength(50)]
        public string? NguoiLienLac { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? DienThoai { get; set; }

        [StringLength(50)]
        public string? DiaChi { get; set; }

        [StringLength(200)]
        public string? MoTa { get; set; }

        public ICollection<Sach> sachs { get; set; }
    }
}
